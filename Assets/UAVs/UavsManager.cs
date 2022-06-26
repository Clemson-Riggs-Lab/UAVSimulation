using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
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
        [SerializeField] private ObjectEventChannelSO uavCreatedChannel = null;
        [SerializeField] private ObjectEventChannelSO uavDisabledChannel = null;
        [NonSerialized] public List<Uav> Uavs = new List<Uav>(); //automatically updated by listening to the uavCreatedChannel

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(uavsContainer,this, this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(waypointsContainer,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(waypointsManager,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(uavsGenerator,this, this.gameObject);

        }
        

        private void OnEnable()
        {
            if(uavCreatedChannel != null)
                uavCreatedChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
            if(uavDisabledChannel != null)
                uavDisabledChannel.Subscribe(OnUavDisabled);
        }

        private void OnUavDisabled(object uav)
        {
            Uavs.Remove((Uav)uav);
        }

        private void OnUavCreated(object uav)
        {
            Uavs.Add((Uav)uav);
            Debug.Log("UAV created"+((Uav)uav).codeName,((Uav)uav).gameObject);
        }


        private void ClearUavs()
        {
            foreach (var uav in Uavs.ToList())
            {
                Destroy(uav.gameObject);
            }
        }

        public List<Uav> GetUavs(bool includeInactive)
        {
            if(includeInactive) return Uavs;
            else return Uavs.Select(uav => uav).Where(uav => uav.isVisuallyEnabled).ToList();
        }
        
        public void GenerateUavs()
        {
            ClearUavs();
            Uavs = uavsGenerator.GenerateOneUAVOnEachWaypoint();//todo check if we need to assign the uavs since we are already listening. if it is not needed, then we can remove the return from the called method as well
        }
        public void GenerateUavs(List<UavRecord> rootObjectUavsRecords)
        {
            ClearUavs();
            Uavs = uavsGenerator.GenerateUavs(rootObjectUavsRecords);
        }
        
        public void NavigateAll()
        {
            foreach (var uav in Uavs)
            {
                uav.Navigate();
            }
        }

        private void OnDisable()
        {
            if(uavCreatedChannel != null)
                uavCreatedChannel.Unsubscribe(OnUavCreated);
        }

       
    }
}


