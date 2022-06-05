using System;
using UAVs;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [Serialize] public GameObject waypointsContainer;
        [Serialize] public GameObject uavsContainer;
        [NonSerialized] private WaypointsManager _waypointsManager;
        [NonSerialized] private UavsManager _uavsManager;
        private void Start()
        {
            _waypointsManager = waypointsContainer.GetComponent<WaypointsManager>();
            _uavsManager = uavsContainer.GetComponent<UavsManager>();
            Invoke(nameof(InitializeSimulation),2);
            
        }

        private void InitializeSimulation()
        {
            _waypointsManager.GenerateWaypoints();
            _uavsManager.GenerateUavs();
        }
    }
}