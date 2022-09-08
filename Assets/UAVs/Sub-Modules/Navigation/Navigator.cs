using System;
using System.Collections.Generic;
using DG.Tweening;
using HelperScripts;
using ScriptableObjects.UAVs.Navigation;
using UnityEngine;

namespace UAVs.Sub_Modules.Navigation
{
	public class Navigator : MonoBehaviour
	{
		private NavigationManager navigationManager;
		private NavigationSettingsSO navigationSettings;

		private UavPathEventChannelSO uavStartedNewPathEventChannel;
		private UavPathEventChannelSO uavArrivedAtDestinationEventChannel;
		
		
		public Uav uav;
		public List<Path> Paths { get; private set; } =new();
		
		//check NavigationSettings for description of below variables
		public NavigationSettingsSO.SpeedMode speedMode;
		public float fixedSpeed;
		public float pathDuration;
		
		public NavigationSettingsSO.FollowType followingType; 
		public float rotationDuration;
		 
		public NavigationSettingsSO.LoopType loopingType; 
		public int numberOfLoops; 
		
		public NavigationSettingsSO.UsedAxis ignorePositionAtAxis;
		
		public NavigationSettingsSO.WaypointHoveringType waypointHoveringType;
		public float hoverDurationOnWaypoint;
		public float hoverAngle;
		public float hoverSpeed;
			
		private Sequence DOTweenSequence;

		private Vector3 TargetPosition => IgnoreWaypointPositionByAxis(_currentPath.DestinationWayPoint.transform.position);
		private Path _currentPath;
		private Path _nextPath;
		private int _pathsIteratorIndex = 0;
		private float _pathStartTime;



		private void Start()
		{
			uav = gameObject.GetComponent<Uav>();
            if (uav == null)
            {
            	Debug.LogError("Navigator: UAV is not attached to the game object on which this script is attached");
            	Destroy(this);
                return;
            }

            InitializeReferences();
            InitializeSettings();
            
            // Fix rigidbody rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
            
            
            
            DOTweenSequence = DOTween.Sequence();
		}

		private void InitializeReferences()
		{
			navigationManager= GameManager.Instance.navigationManager;
			navigationSettings= GameManager.Instance.navigationSettings;
			
			uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavStartedNewPathEventChannel;
			uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavArrivedAtDestinationEventChannel;
			
			AssertionHelper.AssertObjectReferenceObtainedFromGameManager(navigationManager, this, gameObject);
			AssertionHelper.AssertObjectReferenceObtainedFromGameManager(navigationSettings, this, gameObject);
			AssertionHelper.AssertObjectReferenceObtainedFromGameManager(uavStartedNewPathEventChannel, this, gameObject);
			AssertionHelper.AssertObjectReferenceObtainedFromGameManager(uavArrivedAtDestinationEventChannel, this, gameObject);
			
		}

		private void InitializeSettings()
		{
			speedMode = navigationSettings.speedMode;
			fixedSpeed = navigationSettings.fixedSpeed;
			pathDuration = navigationSettings.pathDuration;
			followingType = navigationSettings.followingType;
			rotationDuration = navigationSettings.rotationDuration;
			loopingType = navigationSettings.loopingType;
			numberOfLoops = navigationSettings.numberOfLoops;
			ignorePositionAtAxis = navigationSettings.ignorePositionAtAxis;
			waypointHoveringType = navigationSettings.waypointHoveringType;
			hoverDurationOnWaypoint = navigationSettings.hoverDurationOnWaypoint;
			hoverAngle = navigationSettings.hoverAngle;
			hoverSpeed = navigationSettings.hoverSpeed;
		}
		

		public void StartNavigation()
		{
			if (Paths == null || Paths.Count == 0)
			{
				Debug.LogWarning("No paths attached to UAV" + gameObject.name+ "Navigator Destroyed", gameObject);
				Destroy(this);
			}
			else
			{
				_currentPath = Paths[0];
				
				_nextPath = Paths[1];
				transform.position=_currentPath.StartingWayPoint.transform.position;
				UpdateNavigation();
			}
		}
		

		private Vector3 IgnoreWaypointPositionByAxis(Vector3 transformPosition)
		{ //current position on axis if ignored, otherwise transform position
			var result = transformPosition;
			if (ignorePositionAtAxis.x)
				result.x = transform.position.x;
			if (ignorePositionAtAxis.y)
				result.y = transform.position.y;
			if (ignorePositionAtAxis.z)
				result.z = transform.position.z;
			return result;
		}

		private void UpdateNavigation()
		{	uav.currentPath = _currentPath;
			_currentPath.StartTime= DateTime.Now;
			DOTweenSequence = DOTween.Sequence();
			AddDOTweenNavigationAndRotation();
			uavStartedNewPathEventChannel.RaiseEvent(uav, _currentPath);
			uav.currentPath = _currentPath;
			DOTweenSequence.AppendCallback(()=>uavArrivedAtDestinationEventChannel.RaiseEvent(uav, _currentPath));
			if (waypointHoveringType != NavigationSettingsSO.WaypointHoveringType.None)
			{
				AddHoveringTween();
			}
			
			DOTweenSequence.AppendCallback(UpdateDestination);
			DOTweenSequence.OnComplete(UpdateNavigation);
			
		}

		private void AddHoveringTween()
		{
			switch (waypointHoveringType)
			{
				case NavigationSettingsSO.WaypointHoveringType.UseAngle:
					DOTweenSequence.Append(transform.DORotate(new Vector3(0,hoverAngle,0), hoverDurationOnWaypoint, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
					break;
				case NavigationSettingsSO.WaypointHoveringType.UseSpeed:
					//calculate angles to rotate based on speed and hover duration on waypoint
					var dynamicHoverAngle = hoverDurationOnWaypoint*hoverSpeed;
					DOTweenSequence.Append(transform.DORotate(new Vector3(0,dynamicHoverAngle,0), hoverDurationOnWaypoint, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
					break;
				case NavigationSettingsSO.WaypointHoveringType.FaceNextWaypoint:
					var nextPath = GetPathByIndex(_pathsIteratorIndex + 1);
					if (nextPath != null)
					{
						var nextDestination =nextPath.DestinationWayPoint.transform.position;
						var target= IgnoreWaypointPositionByAxis(nextDestination);
						DOTweenSequence.Append(transform.DOLookAt(target, hoverDurationOnWaypoint).SetEase(Ease.Linear));
					}
					break;
				
				case NavigationSettingsSO.WaypointHoveringType.None:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		private void UpdateDestination()
		{
			var nextPathIndex= _pathsIteratorIndex+1;
			var nextPath = GetPathByIndex(nextPathIndex);
			
			if(nextPath == null)
				Destroy(this);
			else
			{
				_pathsIteratorIndex++;
				_currentPath = nextPath;
				_pathStartTime = Time.time;
			}
			
		}
		
		private Path GetPathByIndex(int pathIndex)
		{
			if (pathIndex % (Paths.Count-1)!=0) //if we are not at the end of the path list
				return Paths[pathIndex%(Paths.Count-1)];
			switch (loopingType) // if we are at the end of the path list
				{
					case NavigationSettingsSO.LoopType.Once:
					default:
						return null;
					case NavigationSettingsSO.LoopType.SeveralTimes: 
						return pathIndex>=numberOfLoops*Paths.Count ? Paths[pathIndex%(Paths.Count-1)] : null; // checking if we have not exceeded the number of loops
					case NavigationSettingsSO.LoopType.Cycled:
						return Paths[pathIndex%(Paths.Count-1)];
				}
		}
		
		private void AddDOTweenNavigationAndRotation()
		{
			var navigationDuration = GetNavigationDuration();

			//Choose or update rotation/facing according to facingType
			switch (followingType)
			{
				// Look at and dampen the rotation
				case NavigationSettingsSO.FollowType.SmoothFacing:
				default: 
					DOTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					DOTweenSequence.Join(transform.DOLookAt(TargetPosition, rotationDuration).SetEase(Ease.Linear));
					break;
				
				// Just Look at	
				case NavigationSettingsSO.FollowType.Facing:
					DOTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					DOTweenSequence.Join(transform.DOLookAt(TargetPosition, 0.1f).SetEase(Ease.Linear));//Quick Rotation
					break;
				
				// Move without rotation
				case NavigationSettingsSO.FollowType.Simple:
					DOTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					break;

				case NavigationSettingsSO.FollowType.Teleport:
					transform.position = TargetPosition;
					break;
				
				// Move and rotate to face the target, with damping 
				case NavigationSettingsSO.FollowType.SmoothDamping:
					DOTweenSequence.Append(transform.DOMove(TargetPosition , navigationDuration).SetEase(Ease.Linear));
					DOTweenSequence.Join(transform.DOLookAt(TargetPosition, rotationDuration).SetEase(Ease.Linear));
					break;
			}
			
		}

		private float GetNavigationDuration()
		{
			var navigationDuration = 0f;
			
			switch (speedMode)
			{
				case NavigationSettingsSO.SpeedMode.FixedSpeed:
					var distance = Vector3.Distance(transform.position, TargetPosition);
					navigationDuration = distance / fixedSpeed;
					break;
				
				case NavigationSettingsSO.SpeedMode.FixedPathDuration:
				default:
					navigationDuration = _pathStartTime+ pathDuration- Time.time;
					break;
				
			}
			return navigationDuration;
		}
	}
}