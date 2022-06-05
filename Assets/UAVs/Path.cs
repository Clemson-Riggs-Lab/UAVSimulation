using System;
using Waypoints;

namespace UAVs
{
    public class Path
    {
        public Waypoint StartingWaypoint;
        public Waypoint DestinationWaypoint;
        
        //fields below are set when the path becomes active and when it is completed.
        [NonSerialized] public DateTime StartTime;
        [NonSerialized] public DateTime EndTime;
    }
}