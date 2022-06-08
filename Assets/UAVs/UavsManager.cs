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
        
        [NonSerialized] public List<Uav> UavsScripts = new List<Uav>();
       

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(uavsContainer,this, this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(waypointsContainer,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(waypointsManager,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(uavsGenerator,this, this.gameObject);

        }
        

        void Start()
        {
            UavsScripts= GetUavsInContainer();
        }
        

        private void ClearUavs()
        {
            foreach (var uavScript in UavsScripts)
            {
                Destroy(uavScript.gameObject);
            }
        }

        private List<Uav> GetUavsInContainer()
        {
            return uavsContainer.GetComponentsInChildren<Uav>().ToList();
        }
        
        public List<Uav> GetUavs(bool includeInactive)
        {
            if(includeInactive) return UavsScripts;
            else return UavsScripts.Select(uav => uav).Where(uav => uav.IsActive).ToList();
        }
        
        public void GenerateUavs()
        {
            ClearUavs();
            UavsScripts = uavsGenerator.GenerateOneUAVOnEachWaypoint();
        }
    }
}


