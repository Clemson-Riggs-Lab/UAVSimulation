using System;
using System.Collections.Generic;
using System.Linq;
using IOHandlers;
using IOHandlers.Records;
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

        private void Start()
        {
            _uavsContainer.transform.position = waypointsContainer.transform.position;
        }

        public List<Uav> GenerateOneUAVOnEachWaypoint()
        {
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
        public List<Uav> GenerateUavs(List<UavRecord> uavsRecords)
        {
            HandleNullValues(uavsRecords);
            var uavs = new List<Uav>();
            foreach (var uavRecord in uavsRecords)
            {
                //linq to get the waypoint by id
                var waypoint = _waypointsManager.waypoints.FirstOrDefault(w => w.Id == uavRecord.StartingWaypointId);
                if(waypoint != null)
                {
                    var uav = GenerateUav(uavRecord.Id??=0, waypoint);
                    uav.SetUavRecord(uavRecord);
                    uavs.Add(uav);
                }
                else
                {
                    Debug.LogError("Waypoint with id " + uavRecord.StartingWaypointId + " not found");
                }
            }
            return uavs;
        }

        private void HandleNullValues(List<UavRecord> uavsRecords)
        {
            var maxId = uavsRecords.Max(x => x.Id) ?? 0;
            foreach (var uavsRecord in uavsRecords)
            {
                uavsRecord.Id ??= maxId + 1;
            }
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
