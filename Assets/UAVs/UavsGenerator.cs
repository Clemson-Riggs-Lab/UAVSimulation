using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace UAVs
{
    public class UavsGenerator : MonoBehaviour
    {
  
        [SerializeField] private GameObject uavPrefab=null;
        [NonSerialized] private GameObject _uavsContainer;
        [SerializeField] private GameObject waypointsContainer;
        [NonSerialized] private WaypointsManager _waypointsManager;
        [NonSerialized] private UavsManager _uavsManager;

        private void Awake()
        {
            _waypointsManager = waypointsContainer.GetComponent<WaypointsManager>();
            /*
             Could just get the uavsContainer by serializing it and attaching the container.
             But since UAVs manager already has it, it is better to get it from there
             This way we can change the container in one place only (the manager), and this script will get it from there
            */
            _uavsManager = gameObject.GetComponent<UavsManager>();
            _uavsContainer = _uavsManager.uavsContainer;   
        }
        

        public List<Uav> GenerateOneUAVOnEachWaypoint()
        {
            _uavsContainer.transform.position = waypointsContainer.transform.position;
            var idIterator= 0;
            var uavs = new List<Uav>();
            
            foreach (var waypoint in _waypointsManager.waypoints)
            {
                var uav = GenerateUav(idIterator, waypoint);
                uavs.Add(uav);
                idIterator++;
            }

            return uavs;
        }

        private Uav GenerateUav(int id, Waypoint waypoint)
        {
            var  uavGO = Instantiate(uavPrefab,  _uavsContainer.transform) as GameObject ;
            var uav= uavGO.GetComponent<Uav>();
            uav.Initialize(id, waypoint);
            uav.ID = id;
        
            return uav;
        }
    }
}
