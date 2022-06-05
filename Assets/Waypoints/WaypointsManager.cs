using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Waypoints
{
    public class WaypointsManager : MonoBehaviour
    {
        [Serialize] public GameObject waypointsContainer;
        [NonSerialized] public List<Waypoint> WaypointsScripts = new List<Waypoint>();
        private WaypointsGenerator _waypointsGenerator;
        
        //events
        public delegate void Notify();
        public event Notify  OnGeneratedWaypointsNotify;

        private void Awake()
        {
            _waypointsGenerator = gameObject.GetComponent<WaypointsGenerator>();
        }

        private void Start()
        {
            WaypointsScripts= GetWaypoints();
        }

        public void GenerateWaypoints()
        {
            ClearWaypoints();
            WaypointsScripts = _waypointsGenerator.GenerateWaypointsUniformOverPlane(16,4,4);
            OnGeneratedWaypointsNotify?.Invoke();
        }
        private void ClearWaypoints()
        {
            foreach (var waypoint in WaypointsScripts)
            {
                Destroy(waypoint.gameObject);
            }
        }

        private List<Waypoint> GetWaypoints()
        {
            return waypointsContainer.GetComponentsInChildren<Waypoint>().ToList();
        }

  
    }
}

