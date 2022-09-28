using System;
using UAVs;
using UnityEngine;
using WayPoints;

namespace Modules.Navigation
{
    public class Path
    {
        [HideInInspector] public WayPoint destinationWayPoint;
        
        [HideInInspector] public int id;
        [HideInInspector] public Uav uav;
        [HideInInspector] public bool uavIsVisuallyEnabled;
        [HideInInspector] public bool targetIsPresent;
        
        public Path previousPath;
        public Path nextPath;
        
        [NonSerialized] public DateTime startTime;
        

        public Path(int id, Uav uav, WayPoint destinationWayPoint, bool visuallyEnabled, bool targetIsPresent)
        {
            this.id = id;
            this.uav = uav;
            this.destinationWayPoint = destinationWayPoint;
            this.uavIsVisuallyEnabled = visuallyEnabled;
            this.targetIsPresent = targetIsPresent;
        }

        public Path(Path path)
        {
            this.id = path.id;
            this.uav = path.uav;
            this.destinationWayPoint = path.destinationWayPoint;
            this.uavIsVisuallyEnabled = path.uavIsVisuallyEnabled;
            this.targetIsPresent = path.targetIsPresent;
            this.previousPath = path.previousPath;
            this.nextPath = path.nextPath;
            this.startTime = path.startTime;
        }
    }
}