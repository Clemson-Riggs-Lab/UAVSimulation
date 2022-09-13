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
        private UavGeneralSettingsSO _uavSettings;
        public List<Uav> uavs = new (); //automatically updated by listening to the uavCreatedEventChannel
        
        public void Initialize()
        {
            GetReferencesFromGameManager();
            SubscribeToChannels();
            _uavsGenerator = gameObject.AddComponent<UavsGenerator>();
            _uavsGenerator.Initialize();
            
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
            _uavSettings = GameManager.Instance.settingsDatabase.uavSettingsDatabase.uavGeneralSettings;
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
            switch (_uavSettings.uavRecordsSource)
            {
                case Enums.InputRecordsSource.FromInputFile:
                    _uavsGenerator.GenerateUavsFromRecords(GameManager.Instance.inputRecordsDatabase.UavsRecords);
                    break;
                case Enums.InputRecordsSource.FromDefaultRecords:
                    _uavsGenerator.GenerateUavsFromRecords(DefaultRecordsCreator.GetDefaultUavRecords());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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


