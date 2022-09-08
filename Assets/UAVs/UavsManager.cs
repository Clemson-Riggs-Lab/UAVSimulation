using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers.Records;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Navigation;
using UAVs.Sub_Modules.Navigation;
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
         
         private UavEventChannelSO uavCreatedEventChannel = null;
         private UavEventChannelSO uavDestroyedEventChannel = null;
         private UavPathEventChannelSO uavStartedNewPathEventChannel;

         public List<Uav> uavs = new (); //automatically updated by listening to the uavCreatedEventChannel
        
        private void Start()
        {
            InitializeChannels();
            SubscribeToChannels();
            GetSettingsFromGameManager();
            AssertReferencesNotNull();
   
            _uavsGenerator = gameObject.AddComponent<UavsGenerator>();
        }

        private void InitializeChannels()
        {
            uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
            uavStartedNewPathEventChannel= GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavStartedNewPathEventChannel;

        }
        private void SubscribeToChannels()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
           
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
            if (uavStartedNewPathEventChannel != null)
            {
                uavStartedNewPathEventChannel.Subscribe(SetUavPath);
            }
        }

        private void SetUavPath(Uav uav, Path path)
        {
            
        }

        private void GetSettingsFromGameManager()
        {
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;
            _wayPointsManager = GameManager.Instance.wayPointsManager;
            _uavsContainer= GameManager.Instance.uavsContainer;
        }

        private void AssertReferencesNotNull()
        {
            AssertionHelper.CheckIfReferenceExistsOrComponentExistsInGameObject(_wayPointsContainer,this, this.gameObject);
            AssertionHelper.CheckIfReferenceExistsOrComponentExistsInGameObject(_wayPointsManager,this, this.gameObject);
            AssertionHelper.CheckIfReferenceExistsOrComponentExistsInGameObject(_uavsContainer,this, this.gameObject);
        }

      
        private void OnUavDestroyed(Uav uav)
        {
            uavs.Remove(uav);
        }

        private void OnUavCreated(Uav uav)
        {
            uavs.Add(uav);
            Debug.Log("UAV created"+uav.CodeName,uav.gameObject);
        }

        private void ClearUavs()
        {
            foreach (var uav in uavs.ToList())
            {
                Destroy(uav.gameObject);
            }
        }

        public List<Uav> GetUavs(bool includeInactive)
        {
            if(includeInactive) 
                return uavs;
            else 
                return uavs.Select(uav => uav).Where(uav => uav.isVisuallyEnabled).ToList();
        }
        
        public void GenerateUavs()
        {
            ClearUavs();
            _uavsGenerator.GenerateOneUAVOnEachWayPoint();
        }
        public void GenerateUavs(List<UavRecord> rootObjectUavsRecords)
        {
            ClearUavs();
            _uavsGenerator.GenerateUavsFromRecords(rootObjectUavsRecords);
        }
        
       

        private void OnDisable()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Unsubscribe(OnUavCreated);
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
        }

       
    }
}


