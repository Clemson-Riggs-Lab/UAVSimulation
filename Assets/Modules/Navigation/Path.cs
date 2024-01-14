using System;
using UAVs;
using UAVs.Settings.ScriptableObjects;
using UnityEngine;
using WayPoints;
using static HelperScripts.Enums;

namespace Modules.Navigation
{
    public class Path
    {
        public WayPoint destinationWayPoint;
        
        public static int iterator= 1;
        public int id;
        public Uav uav;
        public bool uavIsVisuallyEnabledForTargetDetection;
        public bool uavIsVisuallyEnabledForRerouting;
        public bool targetIsPresent;
        public bool nonTargetIsPresent;
        public bool headingToNFZ;
        public bool isReroutePath=false;
        public Path reroutedPath=null;
        public float pathDuration;
        public float timeToReachNFZ=0;
        public DateTime originalStartTimeDateTime;
        public float dynamicStartTime;
        public float originalStartTime;
        public float startTimeForRerouting;
        public ConditionalState OneClickRerouteButtonCondition;


        public Path( Uav uav, WayPoint destinationWayPoint, bool visuallyEnabledForTargetDetection, bool uavIsVisuallyEnabledForRerouting, bool targetIsPresent, bool nonTargetIsPresent,bool headingToNFZ)
        {
            this.id = iterator++;
            this.uav = uav;
            this.destinationWayPoint = destinationWayPoint;
            this.uavIsVisuallyEnabledForTargetDetection = visuallyEnabledForTargetDetection;
            this.uavIsVisuallyEnabledForRerouting = uavIsVisuallyEnabledForRerouting;
            this.targetIsPresent = targetIsPresent;
            this.nonTargetIsPresent = nonTargetIsPresent;
            this.headingToNFZ = headingToNFZ;
            

            if (headingToNFZ)
            {
               pathDuration= timeToReachNFZ = GetCollisionPointWithNFZ();
            }
            else
            {
                pathDuration = Vector3.Distance(uav.transform.position, destinationWayPoint.transform.position) / GameManager.Instance.settingsDatabase.navigationSettings.fixedSpeed;
            }
        }

        private float GetCollisionPointWithNFZ()
        {
            if (Physics.Linecast(uav.transform.position, this.destinationWayPoint.transform.position, out  var hit, 1 << LayerMask.NameToLayer("NFZ")))
            {
                return Vector3.Distance(uav.transform.position, hit.point)/GameManager.Instance.settingsDatabase.navigationSettings.fixedSpeed;
            }
            else return 0;
        }

        public Path(Path path)
        {
            this.id = path.id;
            this.uav = path.uav;
            this.destinationWayPoint = path.destinationWayPoint;
            this.uavIsVisuallyEnabledForTargetDetection = path.uavIsVisuallyEnabledForTargetDetection;
            this.uavIsVisuallyEnabledForRerouting = path.uavIsVisuallyEnabledForRerouting;
            this.targetIsPresent = path.targetIsPresent;
            this.nonTargetIsPresent = path.nonTargetIsPresent;
            this.originalStartTimeDateTime = path.originalStartTimeDateTime;
            this.originalStartTime = path.originalStartTime;
            this.headingToNFZ = path.headingToNFZ;
            this.isReroutePath = true;
            this.reroutedPath = path;
            
            if (headingToNFZ)
            {
                timeToReachNFZ = GetCollisionPointWithNFZ();
            }
            else
            {
                pathDuration = Vector3.Distance(uav.transform.position, destinationWayPoint.transform.position) / GameManager.Instance.settingsDatabase.navigationSettings.fixedSpeed;
            }
        }

        public void SetHeadingToNfz()
        {
            headingToNFZ = (GetCollisionPointWithNFZ() > 0);
        }
    }
}