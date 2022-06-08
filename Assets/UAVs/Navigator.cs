using System.Collections.Generic;
using UnityEngine;

namespace UAVs
{
    public class Navigator : MonoBehaviour
    {
        public List<waypoint>[] waypoints;
        public float speed = 1;
        
       
        public List<Path> NavigateInSequence( int numberOfBounces)
        {
            //move in the following order: 0,1,2,3,7,11,15,14,13,12,8,9,10,6,5,4,0, repeat
	        public int[] waypointOrder = {0,1,2,3,7,11,15,14,13,12,8,9,10,6,5,4};
            
            List<Path> paths = new List<Path>();
            
            
        }


    }
}