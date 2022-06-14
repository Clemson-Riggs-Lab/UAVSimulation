using System;
using System.Collections.Generic;
using System.Linq;
using Helper_Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace UAVs
{
    public class UavsManager : MonoBehaviour
    {
        [SerializeField] public GameObject uavsContainer;
        [SerializeField] public GameObject waypointsContainer;
        [SerializeField] private WaypointsManager waypointsManager;
        [SerializeField] private UavsGenerator uavsGenerator;
        
        [NonSerialized] public List<Uav> Uavs = new List<Uav>();

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(uavsContainer,this, this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(waypointsContainer,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(waypointsManager,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(uavsGenerator,this, this.gameObject);

        }
        

        void Start()
        {
            Uavs= GetUavsInContainer();
        }
        

        private void ClearUavs()
        {
            foreach (var uav in Uavs)
            {
                Destroy(uav.gameObject);
            }
        }

        private List<Uav> GetUavsInContainer()
        {
            return uavsContainer.GetComponentsInChildren<Uav>().ToList();
        }
        
        public List<Uav> GetUavs(bool includeInactive)
        {
            if(includeInactive) return Uavs;
            else return Uavs.Select(uav => uav).Where(uav => uav.isActive).ToList();
        }
        
        public void GenerateUavs()
        {
            ClearUavs();
            Uavs = uavsGenerator.GenerateOneUAVOnEachWaypoint();
        }
        
        public void NavigateAll()
        {
            foreach (var uav in Uavs)
            {
                uav.Navigate();
            }
        }
    }
}


