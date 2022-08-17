using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using UAVs.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;

namespace UAVs
{
    public class UavsManager : MonoBehaviour
    {
         private GameObject _uavsContainer;
         private GameObject _wayPointsContainer;
         private WayPointsManager _wayPointsManager;
         private UavsGenerator _uavsGenerator;
         
        [SerializeField] private ObjectEventChannelSO uavCreatedChannel = null;
        [SerializeField] private ObjectEventChannelSO uavDisabledChannel = null;
        [NonSerialized] public List<Uav> Uavs = new List<Uav>(); //automatically updated by listening to the uavCreatedChannel
 
        
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

        private void Start()
        {
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;
            _wayPointsManager = GameManager.Instance.wayPointsManager;
            _uavsContainer= GameManager.Instance.uavsContainer;
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(_wayPointsContainer,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(_wayPointsManager,this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(_uavsContainer,this, this.gameObject);
            
            _uavsGenerator = gameObject.AddComponent<UavsGenerator>();
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
            if(includeInactive) 
                return Uavs;
            else 
                return Uavs.Select(uav => uav).Where(uav => uav.isVisuallyEnabled).ToList();
        }
        
        public void GenerateUavs()
        {
            ClearUavs();
            _uavsGenerator.GenerateOneUAVOnEachWayPoint();
        }
        public void GenerateUavs(List<UavRecord> rootObjectUavsRecords)
        {
            ClearUavs();
            _uavsGenerator.GenerateUavs(rootObjectUavsRecords);
        }
        
       

        private void OnDisable()
        {
            if(uavCreatedChannel != null)
                uavCreatedChannel.Unsubscribe(OnUavCreated);
        }

       
    }
}


