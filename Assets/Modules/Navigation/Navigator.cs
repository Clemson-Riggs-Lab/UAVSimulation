using System;
using System.Collections.Generic;
using DG.Tweening;
using HelperScripts;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums.InputRecordsSource;
using static HelperScripts.Enums.UavCondition;
using Sequence = DG.Tweening.Sequence;

namespace Modules.Navigation
{
	public class Navigator : MonoBehaviour
	{
		[NonSerialized] private NavigationManager _navigationManager;
		[NonSerialized] public NavigationSettingsSO navigationSettings;

		[NonSerialized] private UavPathEventChannelSO _uavStartedNewPathEventChannel;
		[NonSerialized] private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		[NonSerialized] private UavConditionEventChannelSO _uavConditionChangedEventChannel;

		[NonSerialized] private Uav _uav;
		[NonSerialized] private GameObject _uavBody;
		
		[NonSerialized] private Sequence _doTweenSequence;

		private Vector3 TargetPosition
		{
			get
			{
				if (_currentPath == null || _currentPath.destinationWayPoint == null)
				{
					Debug.LogError("Navigator: Current path or destination waypoint is null. Current path is " + _currentPath + " and destination waypoint is " + _currentPath.destinationWayPoint, this);
					return transform.position;
				}
				else
				{
					return IgnoreWaypointPositionByAxis(_currentPath.destinationWayPoint.transform.position);
				}
			}
		}
		
		[NonSerialized] private Path _currentPath;

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
			
			_uav.currentPath=_currentPath = _navigationManager.GetRandomPathDynamically(_uav);
			_currentPath.startTime = DateTime.Now;
			UpdateNavigation();
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
		/// It bypasses the StartNextDestination method and directly sets the new destination, meaning that the _pathStartTime is not updated
		/// This allows us to keep all uavs in sync even when we reroute one of them.
		/// </summary>
		/// <param name="newPath"></param>
		public void Reroute(Path newPath)
		{
			_uav.currentPath=_currentPath = newPath;
			_doTweenSequence.Kill();
			UpdateNavigation(true);
		}
		
		private void UpdateNavigation(bool reroute=false)
		{
			if(_uav.uavCondition== Lost) return; //we stop the navigation if the UAV is lost, since this is called recursively, this suffices to stop the navigation.
			//also note that the above is just a safety check, since the navigator should be destroyed by the navigation manager when the UAV is lost.
			_doTweenSequence = DOTween.Sequence();
			_doTweenSequence.AppendCallback(()=>_uavStartedNewPathEventChannel.RaiseEvent(_uav, _currentPath));
			if(!reroute)
				AddHoveringTween();
			AddDOTweenNavigationAndRotation();
			_doTweenSequence.AppendCallback(()=>_uavArrivedAtDestinationEventChannel.RaiseEvent(_uav, _currentPath));
			
			_doTweenSequence.OnComplete(StartNextDestination);
		}
		
		private void StartNextDestination()
		{
			_uav.currentPath=_currentPath=_navigationManager.GetRandomPathDynamically(_uav);
			_currentPath.startTime= DateTime.Now;
			
			if(_currentPath == null) // we have reached the end 
			{
				Destroy(this);
				Debug.Log("We couldnt find a new path for " + _uav.name + " so we are destroying the navigator");
			}			
			else
			{
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
			if(navigationDuration == 0) return; //we don't want to add a tween if the duration is 0.
			
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
					if (_currentPath != null && _currentPath.destinationWayPoint != null)  // else if it is null then we do not have to rotate anymore as there is no next waypoint to face
					{
						//if already facing the next waypoint, then we do not need to rotate
						var nextWaypointPosition = _currentPath.destinationWayPoint.transform.position;
						var angle = Vector3.Angle(_uavBody.transform.forward, (nextWaypointPosition - transform.position).normalized);
						if (angle >= 10f)
							return;
						
						else
							_doTweenSequence.Append(_uavBody.transform.DOLookAt(TargetPosition, navigationSettings.hoverDurationOnWaypoint, AxisConstraint.None, Vector3.up ).SetEase(Ease.Linear));
					}
					break;
					
				case NavigationSettingsSO.WaypointHoveringType.None:
				default:
					return;
			}
		}
		
		private float GetNavigationDuration()
		{
			var navigationDuration = 0f;
			var distance = Vector3.Distance(transform.position, TargetPosition);
			navigationDuration = distance / navigationSettings.fixedSpeed;
			return navigationDuration;
		}

		private void OnDestroy()
		{
			if(_doTweenSequence!=null)
				_doTweenSequence.Kill();
		}
	}
}