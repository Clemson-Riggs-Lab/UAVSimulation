using System;
using System.Collections.Generic;
using System.Linq;
using IOHandlers;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UAVs.Channels.ScriptableObjects;
using UAVs.Settings.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;
using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;

namespace UAVs
{
    public class UavsManager : MonoBehaviour
    {
        private UavsGenerator _uavsGenerator;
        private UavEventChannelSO _uavCreatedEventChannel;
        private UavEventChannelSO _uavDestroyedEventChannel;
        private UavPathEventChannelSO _uavStartedNewPathEventChannel;
        private UavConditionEventChannelSO _uavConditionChangedEventChannel;
        private VoidEventChannelSO _simulationEndedEventChannel;
        private UavSettingsSO _uavSettings;
        public List<Uav> uavs = new (); //automatically updated by listening to the uavCreatedEventChannel
        public List<Uav> lostAndFinishedUavs = new ();  //automatically updated by monitoring the uavConditionChangedEventChannel
        
        public void Initialize()
        {
            GetReferencesFromGameManager();
            SubscribeToChannels();
            
            //logging
            var uavLogHandler = gameObject.GetOrAddComponent<UavLogHandler>();
            uavLogHandler.Initialize();
            
            //uav generator
            _uavsGenerator = gameObject.GetOrAddComponent<UavsGenerator>();
            _uavsGenerator.Initialize();
            GenerateUavs();
        }
        private void SubscribeToChannels()
        {
            if(_uavCreatedEventChannel != null)
                _uavCreatedEventChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
           
            if(_uavDestroyedEventChannel != null)
                _uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
            
            if(_uavConditionChangedEventChannel != null)
                _uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
            
            if(_uavStartedNewPathEventChannel != null)
                _uavStartedNewPathEventChannel.Subscribe(OnUavStartedNewPath);

        }

        private void OnUavConditionChanged(Uav uav, UavCondition uavCondition)
        {
            if (!uavs.Contains(uav))
            {
                Debug.LogError("UAV not found in the list of uavs in the UavsManager");
                return;
            }
            uav.uavCondition = uavCondition;
            if ((uavCondition is Lost or Finished )&& !lostAndFinishedUavs.Contains(uav))
            {
                lostAndFinishedUavs.Add(uav);
                
                if (lostAndFinishedUavs.Count == uavs.Count)
                {
                    _simulationEndedEventChannel.RaiseEvent();
                }
            }

            SetUavVisibilityAndCollidability(uav, uavCondition);
            //Hide/Show uav based on its condition, and set collisions active or not
          
            
            
        }

        private void SetUavVisibilityAndCollidability(Uav uav, UavCondition uavCondition)
        {
            // visibility
            if((_uavSettings.hideUavInMapWhenHidden && uavCondition is Hidden)  ||(_uavSettings.hideUavInMapWhenLostOrFinished && uavCondition is Lost or Finished))
                uav.SetVisibility(false);
            
            else
                uav.SetVisibility(true);
            
            //collisions
            if((_uavSettings.disableCollisionWithNFZWhenHidden && uavCondition is Hidden))
                uav.SetCollisions(false);
            
            else
                uav.SetCollisions(true);
        }

        private void OnUavStartedNewPath(Uav uav, Path path)
        {
            if (!uavs.Contains(uav))
            {
                Debug.LogError("UAV not found in the list of uavs in the UavsManager");
                return;
            }
            uav.currentPath = path;
            if (uav.uavCondition!=Lost)
            {
                _uavConditionChangedEventChannel.RaiseEvent(uav, path.uavIsVisuallyEnabled ? Enabled : Hidden);
            }
               
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
                case InputRecordsSource.FromInputFile:
                    _uavsGenerator.GenerateUavsFromRecords(GameManager.Instance.inputRecordsDatabase.UavsRecords);
                    break;
                case InputRecordsSource.FromDefaultRecords:
                    _uavsGenerator.GenerateUavsFromRecords(DefaultRecordsCreator.GetDefaultUavRecords());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        private void OnUavDestroyed(Uav uav)
        {
            uavs.Remove(uav);
        }

        private void OnUavCreated(Uav uav)
        {
            uavs.Add(uav);
            //Debug.Log(uav.uavName + " Created",uav.gameObject);
        }

        private void GetReferencesFromGameManager()
        {
            _uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            _uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
            _uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
            _uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
            _simulationEndedEventChannel = GameManager.Instance.channelsDatabase.simulationEndedEventChannel;
            _uavSettings = GameManager.Instance.settingsDatabase.uavSettings;
            
        }

        private void OnDisable()
        {
            UnsubscribeFromChannels();
        }

        private void UnsubscribeFromChannels()
        {
            if(_uavCreatedEventChannel != null)
                _uavCreatedEventChannel.Unsubscribe(OnUavCreated);
            if(_uavDestroyedEventChannel != null)
                _uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
            if(_uavConditionChangedEventChannel != null)
                _uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);
            if(_uavStartedNewPathEventChannel != null)
                _uavStartedNewPathEventChannel.Unsubscribe(OnUavStartedNewPath);
        }

        public Uav GetUAVAgainstId(int uavId)
        {
            return uavs.Find(x => x.id == uavId);
        }
    }
}


