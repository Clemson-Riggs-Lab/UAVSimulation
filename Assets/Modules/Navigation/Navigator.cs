using System;
using System.Collections.Generic;
using DG.Tweening;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums.UavCondition;
using Sequence = DG.Tweening.Sequence;

namespace Modules.Navigation
{
	public class Navigator : MonoBehaviour
	{
		public static event Action UpdatingNavigation_Event;

		[NonSerialized] private NavigationManager _navigationManager;
		[NonSerialized] public NavigationSettingsSO navigationSettings;

		[NonSerialized] private UavPathEventChannelSO _uavStartedNewPathEventChannel;
		[NonSerialized] private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		[NonSerialized] private UavConditionEventChannelSO _uavConditionChangedEventChannel;

		[NonSerialized] private Uav _uav;
		[NonSerialized] private GameObject _uavBody;
		[NonSerialized] public List<Path> paths =new();
		
		[NonSerialized] private Sequence _doTweenSequence;

		 private Vector3 TargetPosition => IgnoreWaypointPositionByAxis(_currentPath.destinationWayPoint.transform.position);
		[NonSerialized] private Path _currentPath;
		[NonSerialized] private float _pathStartTime; // this is used instead of _currentPath.startTime because this records simulation time and not real time ( seconds hours date month year etc )



		private void Start()
		{
			InitializeReferences();
			_doTweenSequence = DOTween.Sequence();
		}

		private void InitializeReferences()
		{
			_uav = gameObject.GetComponent<Uav>();
			_uavBody = _uav.uavBody;
			if (GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().freezeRotation = true;
			if (_uav == null)
			{
				Debug.LogError("Navigator: UAV is not attached to the game object on which this script is attached");
				Destroy(this);
				return;
			}
			_navigationManager= GameManager.Instance.navigationManager;
			navigationSettings= GameManager.Instance.settingsDatabase.navigationSettings;
			
			_uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
			_uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;

		}
		
		

		public void StartNavigation()
		{
			if (paths.Count == 0)
			{
				Debug.LogWarning("No paths attached to UAV" + gameObject.name+ "Navigator Destroyed", gameObject);
				Destroy(this);
			}
			else
			{
				_currentPath = paths[0];
				_currentPath.startTime = DateTime.Now;
				_pathStartTime = Time.time;
				UpdateNavigation();
			}
		}
		

		private Vector3 IgnoreWaypointPositionByAxis(Vector3 transformPosition)
		{ //current position on axis if axis is ignored, otherwise transform position 
			var result = transformPosition;
			if (navigationSettings.ignoreWaypointPositionOnAxis.x)
				result.x = transform.position.x;
			if (navigationSettings.ignoreWaypointPositionOnAxis.y)
				result.y = transform.position.y;
			if (navigationSettings.ignoreWaypointPositionOnAxis.z)
				result.z = transform.position.z;
			return result;
		}

		/// <summary>
		/// This method is called when we want to change the current path to a new destination (e.g., when we are re-routing the UAV)
		/// It bypasses the GetNextDestination method and directly sets the new destination, meaning that the _pathStartTime is not updated
		/// This allows us to keep all uavs in sync even when we reroute one of them.
		/// </summary>
		/// <param name="newPath"></param>
		public void Reroute(Path newPath)
        {
            _currentPath = newPath;
			_doTweenSequence.Kill();
			UpdateNavigation();
		}
		
		private void UpdateNavigation()
        {
			UpdatingNavigation_Event?.Invoke();
            if (_uav.uavCondition== Lost) return; //we stop the navigation if the UAV is lost, since this is called recursively, this suffices to stop the navigation.
			//also note that the above is just a safety check, since the navigator should be destroyed by the navigation manager when the UAV is lost.
			_uavStartedNewPathEventChannel.RaiseEvent(_uav, _currentPath);
			_doTweenSequence = DOTween.Sequence();
			AddDOTweenNavigationAndRotation();
			_doTweenSequence.AppendCallback(()=>_uavArrivedAtDestinationEventChannel.RaiseEvent(_uav, _currentPath));
			if (navigationSettings.waypointHoveringType != NavigationSettingsSO.WaypointHoveringType.None)
			{
				AddHoveringTween();
			}
			_doTweenSequence.OnComplete(GetNextDestination);
		}
		
		private void GetNextDestination()
		{
			_currentPath= _currentPath.nextPath;
			_currentPath.startTime= DateTime.Now;
			
			if(_currentPath == null) // we have reached the end 
			{
				Destroy(this);
				_uavConditionChangedEventChannel.RaiseEvent(_uav, Finished);
			}			
			else
			{
				_pathStartTime = Time.time;
				UpdateNavigation();
			}
			
		}
		
		/// <summary>
		/// This method is responsible for adding tweens of navigation and rotation. It does not modify the destination or any other path related data.
		/// It only gets the current path and adds tweens to the DOTweenSequence.
		/// </summary>
		private void AddDOTweenNavigationAndRotation() 
		{
			var navigationDuration = GetNavigationDuration();

			//Choose or update rotation/facing according to facingType
			switch (navigationSettings.followingType)
			{
				// Look at and dampen the rotation
				case NavigationSettingsSO.FollowType.SmoothFacing:
				default: 
					_doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					_doTweenSequence.Join(_uavBody.transform.DOLookAt(TargetPosition, navigationSettings.rotationDuration, AxisConstraint.Y).SetEase(Ease.Linear));
					break;
				
				// Just Look at	
				case NavigationSettingsSO.FollowType.Facing:
					_doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					_doTweenSequence.Join(_uavBody.transform.DOLookAt(TargetPosition, 0.1f, AxisConstraint.Y).SetEase(Ease.Linear));//Quick Rotation
					break;
				
				// Move without rotation
				case NavigationSettingsSO.FollowType.Simple:
					_doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					break;

				case NavigationSettingsSO.FollowType.Teleport:
					transform.position = TargetPosition;
					break;
				
				// Move and rotate to face the target, with damping 
				case NavigationSettingsSO.FollowType.SmoothDamping:
					_doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					_doTweenSequence.Join(_uavBody.transform.DOLookAt(TargetPosition, navigationSettings.rotationDuration, AxisConstraint.Y).SetEase(Ease.Linear));
					break;
			}
			
		}

		/// <summary>
		///  Adds tweens to the DOTween sequence for rotating the UAV while it is over the waypoint
		/// This method does not  modify anything related to the path or the destination
		/// </summary>
		private void AddHoveringTween()
		{
			switch (navigationSettings.waypointHoveringType)
			{
				case NavigationSettingsSO.WaypointHoveringType.UseAngle:
					_doTweenSequence.Append(_uavBody.transform.DORotate(new Vector3(0,navigationSettings.hoverAngle,0), navigationSettings.hoverDurationOnWaypoint, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
					break;
				case NavigationSettingsSO.WaypointHoveringType.UseSpeed:
					//calculate angles to rotate based on speed and hover duration on waypoint
					var dynamicHoverAngle = navigationSettings.hoverDurationOnWaypoint*navigationSettings.hoverSpeed;
					_doTweenSequence.Append(_uavBody.transform.DORotate(new Vector3(0,dynamicHoverAngle,0), navigationSettings.hoverDurationOnWaypoint, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
					break;
				case NavigationSettingsSO.WaypointHoveringType.FaceNextWaypoint :
					if (_currentPath.nextPath != null) // else if it is null then we do not have to rotate anymore as there is no next waypoint to face
					{
						var target= IgnoreWaypointPositionByAxis(_currentPath.nextPath.destinationWayPoint.transform.position);
						// use dotween Dorotate  to rotate the UAV to face the target
						_doTweenSequence.Append(_uavBody.transform.DOLookAt(target, navigationSettings.hoverDurationOnWaypoint, AxisConstraint.Y, Vector3.up ).SetEase(Ease.Linear));
						//TODO fix this so that we dont have wobbly rotation
					}
					break;
					
				case NavigationSettingsSO.WaypointHoveringType.None:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		/// <summary>
		/// This method is responsible for calculating the duration of the navigation tween.
		/// It does not modify the destination or any other path related data.
		///  It calculates the duration based on the current path and the speed of the object. duration = distance/speed
		/// Or it uses the duration of the path if it is set.
		/// Note that for rerouting, the pathStartTime would be set from the previous path,
		/// so the lapsed duration would be subtracted from the fixed duration to give us the remaining duration.
		/// </summary>
		/// 
		/// <example>
		/// fixed duration: fixed duration=10 sec.
		/// path started at t=50 sec,
		/// at t=52 we rerouted,
		/// then the result would be 10+50-52 = 8 sec
		/// </example>
		/// 
		/// <example>
		/// fixed speed: Same example as above,
		/// but speed fixed to 10 units/sec.
		/// We do not care if it is re-routing or not,
		/// we calculate the duration directly by dividing the distance over the speed
		/// </example>
		private float GetNavigationDuration()
		{
			var navigationDuration = 0f;
			
			switch (navigationSettings.speedMode)
			{
				case NavigationSettingsSO.SpeedMode.FixedSpeed:
					var distance = Vector3.Distance(transform.position, TargetPosition);
					navigationDuration = distance / navigationSettings.fixedSpeed;
					break;
				
				case NavigationSettingsSO.SpeedMode.FixedPathDuration:
				default:
					navigationDuration = navigationSettings.pathDuration+_pathStartTime-Time.time;
					break;
				
			}
			return navigationDuration;
		}

		private void OnDestroy()
		{
			if(_doTweenSequence!=null)
				_doTweenSequence.Kill();
		}
	}
}