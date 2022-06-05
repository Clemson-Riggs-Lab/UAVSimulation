using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace UAVs
{
    public class UavsManager : MonoBehaviour
    {
        [Serialize] public GameObject uavsContainer;
        [Serialize] public GameObject waypointsContainer;
        [NonSerialized] public List<Uav> UavsScripts = new List<Uav>();
        [NonSerialized] private WaypointsManager _waypointsManager;
        private UavsGenerator _uavsGenerator;

        private void Awake()
        {
            _uavsGenerator = gameObject.GetComponent<UavsGenerator>();
            _waypointsManager = waypointsContainer.GetComponent<WaypointsManager>();

        }

        void Start()
        {
            UavsScripts= GetUavs();
        }
        

        private void ClearUavs()
        {
            foreach (var uavScript in UavsScripts)
            {
                Destroy(uavScript.gameObject);
            }
        }

        private List<Uav> GetUavs()
        {
            return uavsContainer.GetComponentsInChildren<Uav>().ToList();
        }

        public void GenerateUavs()
        {
            ClearUavs();
            UavsScripts = _uavsGenerator.GenerateOneUAVOnEachWaypoint();
        }
    }
}


