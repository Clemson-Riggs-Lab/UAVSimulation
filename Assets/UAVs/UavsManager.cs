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
        private UavsGenerator _uavsGenerator;
        private UavEventChannelSO uavCreatedEventChannel = null;
        private UavEventChannelSO uavDestroyedEventChannel = null;
        public List<Uav> uavs = new (); //automatically updated by listening to the uavCreatedEventChannel
        
        private void Start()
        {
            GetReferencesFromGameManager();
            SubscribeToChannels();
            _uavsGenerator = gameObject.AddComponent<UavsGenerator>();
        }
        
        private void SubscribeToChannels()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
           
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Subscribe(OnUavDestroyed);

        }



        private void GetReferencesFromGameManager()
        {
            uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
        }
        
      
        private void OnUavDestroyed(Uav uav)
        {
            uavs.Remove(uav);
        }

        private void OnUavCreated(Uav uav)
        {
            uavs.Add(uav);
            Debug.Log(uav.uavName + " Created",uav.gameObject);
        }

        private void ClearUavs()
        {
            foreach (var uav in uavs.ToList())
            {
                Destroy(uav.gameObject);
            }
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


