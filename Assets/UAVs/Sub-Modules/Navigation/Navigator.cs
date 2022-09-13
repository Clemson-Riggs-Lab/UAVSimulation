using System;
using System.Collections.Generic;
using DG.Tweening;
using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace UAVs.Sub_Modules.Navigation
{
	public class Navigator : MonoBehaviour
	{
		[DoNotSerialize] private NavigationManager navigationManager;
		[DoNotSerialize] public NavigationSettingsSO navigationSettings;

		[DoNotSerialize] private UavPathEventChannelSO uavStartedNewPathEventChannel;
		[DoNotSerialize] private UavPathEventChannelSO uavArrivedAtDestinationEventChannel;

		[DoNotSerialize] private Uav uav;
		[DoNotSerialize] private GameObject uavBody;
		[DoNotSerialize] public List<Path> Paths { get; private set; } =new();
		
		[DoNotSerialize] private Sequence doTweenSequence;

		[DoNotSerialize] private Vector3 TargetPosition => IgnoreWaypointPositionByAxis(_currentPath.destinationWayPoint.transform.position);
		[DoNotSerialize] private Path _currentPath;
		[DoNotSerialize] private float _pathStartTime;



		private void Start()
		{
			uav = gameObject.GetComponent<Uav>();
			uavBody = uav.uavBody;
            if (uav == null)
            {
            	Debug.LogError("Navigator: UAV is not attached to the game object on which this script is attached");
            	Destroy(this);
                return;
            }

            InitializeReferences();
            
            // Fix rigidbody rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
            
            
            
            doTweenSequence = DOTween.Sequence();
		}

		private void InitializeReferences()
		{
			navigationManager= GameManager.Instance.navigationManager;
			navigationSettings= GameManager.Instance.settingsDatabase.uavSettingsDatabase.navigationSettings;
			
			uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavStartedNewPathEventChannel;
			uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavArrivedAtDestinationEventChannel;

		}
		
		

		public void StartNavigation()
		{
			if (Paths.Count == 0)
			{
				Debug.LogWarning("No paths attached to UAV" + gameObject.name+ "Navigator Destroyed", gameObject);
				Destroy(this);
			}
			else
			{
				_currentPath = Paths[0];
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
			doTweenSequence.Kill();
			UpdateNavigation();
		}
		
		private void UpdateNavigation()
		{	uav.currentPath = _currentPath;
			_currentPath.startTime= DateTime.Now;
			uavStartedNewPathEventChannel.RaiseEvent(uav, _currentPath);
			doTweenSequence = DOTween.Sequence();
			AddDOTweenNavigationAndRotation();
			doTweenSequence.AppendCallback(()=>uavArrivedAtDestinationEventChannel.RaiseEvent(uav, _currentPath));
			if (navigationSettings.waypointHoveringType != NavigationSettingsSO.WaypointHoveringType.None)
			{
				AddHoveringTween();
			}
			
			doTweenSequence.AppendCallback(GetNextDestination);
			doTweenSequence.OnComplete(UpdateNavigation);
			
		}
		
		private void GetNextDestination()
		{
			_currentPath= _currentPath.nextPath;

			if(_currentPath == null) // we have reached the end 
				//TODO raise event that uav finished navigation
				Destroy(this);
			else
			{
				_pathStartTime = Time.time;
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
					doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					doTweenSequence.Join(uavBody.transform.DOLookAt(TargetPosition, navigationSettings.rotationDuration).SetEase(Ease.Linear));
					break;
				
				// Just Look at	
				case NavigationSettingsSO.FollowType.Facing:
					doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					doTweenSequence.Join(uavBody.transform.DOLookAt(TargetPosition, 0.1f).SetEase(Ease.Linear));//Quick Rotation
					break;
				
				// Move without rotation
				case NavigationSettingsSO.FollowType.Simple:
					doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					break;

				case NavigationSettingsSO.FollowType.Teleport:
					transform.position = TargetPosition;
					break;
				
				// Move and rotate to face the target, with damping 
				case NavigationSettingsSO.FollowType.SmoothDamping:
					doTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					doTweenSequence.Join(uavBody.transform.DOLookAt(TargetPosition, navigationSettings.rotationDuration).SetEase(Ease.Linear));
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
					doTweenSequence.Append(uavBody.transform.DORotate(new Vector3(0,navigationSettings.hoverAngle,0), navigationSettings.hoverDurationOnWaypoint, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
					break;
				case NavigationSettingsSO.WaypointHoveringType.UseSpeed:
					//calculate angles to rotate based on speed and hover duration on waypoint
					var dynamicHoverAngle = navigationSettings.hoverDurationOnWaypoint*navigationSettings.hoverSpeed;
					doTweenSequence.Append(uavBody.transform.DORotate(new Vector3(0,dynamicHoverAngle,0), navigationSettings.hoverDurationOnWaypoint, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
					break;
				case NavigationSettingsSO.WaypointHoveringType.FaceNextWaypoint :
					if (_currentPath.nextPath != null) // else if it is null then we do not have to rotate anymore as there is no next waypoint to face
					{
						var target= IgnoreWaypointPositionByAxis(_currentPath.nextPath.destinationWayPoint.transform.position);
						
							doTweenSequence.Append(uavBody.transform.DOLookAt(target, navigationSettings.hoverDurationOnWaypoint).SetEase(Ease.Linear));
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
			if(doTweenSequence!=null)
				doTweenSequence.Kill();
		}
	}
}