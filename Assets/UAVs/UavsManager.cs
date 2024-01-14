using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using Modules.TargetDetection.Settings.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UAVs.Channels.ScriptableObjects;
using UAVs.Settings.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;

using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;

namespace UAVs
{
    public class UavsManager : MonoBehaviour
    {
        private UavsGenerator _uavsGenerator;
        private NavigationManager _navigationManager;
        private UavEventChannelSO _uavCreatedEventChannel;
        private UavEventChannelSO _uavDestroyedEventChannel;
        private UavPathEventChannelSO _uavStartedNewPathEventChannel;
        private UavConditionEventChannelSO _uavConditionChangedEventChannel;
        private VoidEventChannelSO _simulationEndedEventChannel;
        private UavSettingsSO _uavSettings;
        private NavigationSettingsSO _navigationSettings;
        private TargetDetectionSettingsSO _targetDetectionSettings;
        public List<Uav> uavs = new (); //automatically updated by listening to the uavCreatedEventChannel
        public List<Uav> lostUavs = new ();  //automatically updated by monitoring the uavConditionChangedEventChannel
       
        private static List<int> availableLayers = new List<int>();
        public void Initialize()
        {
            GetReferencesFromGameManager();
            SubscribeToChannels();
            
            
            //initialize available layers
            for (int i = 0; i <= 20; i++)
            {
                availableLayers.Add(LayerMask.NameToLayer("UAV" + i));
            }

            //logging
            var uavLogHandler = gameObject.GetOrAddComponent<UavLogHandler>();
            uavLogHandler.Initialize();
            
            //uav generator
            _uavsGenerator = gameObject.GetOrAddComponent<UavsGenerator>();
            _uavsGenerator.Initialize();
            GenerateUavs();
            _navigationManager.Initialize();
            CheckIfNeedToGenerateMoreUavs();
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
            uav.uavCondition =uavCondition;
            if ( uavCondition == Lost && !lostUavs.Contains(uav))
            {
                lostUavs.Add(uav);
                CheckIfNeedToGenerateMoreUavs();
            }

            SetUavVisibilityAndCollidability(uav, uavCondition);
        }

        private void CheckIfNeedToGenerateMoreUavs()
        {
            var numberOfActiveUavsForRerouting = 0;
            var numberOfActiveUavsForTargetDetection = 0;
            
            if (!_navigationSettings.distinctUavsForReroutingAndTargetDetection)
            { 
                numberOfActiveUavsForRerouting = _navigationSettings.numberOfActiveUavsForRerouting - (uavs.Count - lostUavs.Count);
                numberOfActiveUavsForTargetDetection = _targetDetectionSettings.numberOfActiveUavsForTargetDetection - (uavs.Count - lostUavs.Count);
            }
            else //if distinct uavs are needed for 
            {
                numberOfActiveUavsForRerouting = _navigationSettings.numberOfActiveUavsForRerouting - (uavs.Count - (lostUavs.Count+_targetDetectionSettings.numberOfActiveUavsForTargetDetection));
                numberOfActiveUavsForTargetDetection = _targetDetectionSettings.numberOfActiveUavsForTargetDetection - (uavs.Count - (lostUavs.Count+_navigationSettings.numberOfActiveUavsForRerouting));
            }

            var numberOfUavsToGenerate = Math.Max(numberOfActiveUavsForRerouting, numberOfActiveUavsForTargetDetection);

            if (numberOfUavsToGenerate > 0)
            {
                var generatedUavs = _uavsGenerator.GenerateUavsRandomly(numberOfUavsToGenerate);
                StartCoroutine(_navigationManager.InitializeUavs(generatedUavs));
                
                RemoveUavsFromLostUavs(generatedUavs.Count);
            }
            
        }

        private void RemoveUavsFromLostUavs(int numberOfUavsToRemove)
        {
            if (lostUavs.Count<numberOfUavsToRemove)
                numberOfUavsToRemove = lostUavs.Count;
            for (var i = 0; i < numberOfUavsToRemove; i++)
            {
                var  randomIndex = UnityEngine.Random.Range(0, lostUavs.Count);
                var uav = lostUavs[randomIndex];
                lostUavs.RemoveAt(randomIndex);
                uavs.Remove(uav);
                Destroy(uav.gameObject);
            }
        }

        private void SetUavVisibilityAndCollidability(Uav uav, UavCondition uavCondition)
        {
            // visibility
            if((_uavSettings.hideUavInMapWhenHidden && uavCondition is Hidden or EnabledForTargetDetectionOnly)  ||(_uavSettings.hideUavInMapWhenLost && uavCondition is Lost))
                uav.SetVisibility(false);
            
            else
                uav.SetVisibility(true);
            
            //collisions
            if((_uavSettings.disableCollisionWithNFZWhenHidden && uavCondition is Hidden or EnabledForTargetDetectionOnly))
                uav.SetCollisions(false);
            
            else
                uav.SetCollisions(true);
        }

        private void OnUavStartedNewPath(Uav uav, Path path)
        {
            uav.currentPath = path;
            if (uav.uavCondition == Lost) return;
            
            var condition = Hidden;

            if (path.uavIsVisuallyEnabledForTargetDetection && path.uavIsVisuallyEnabledForRerouting)
            {
                condition = EnabledForTargetDetectionAndRerouting;
            }
            else if (path.uavIsVisuallyEnabledForTargetDetection)
            {
                condition = EnabledForTargetDetectionOnly;
            }
            else if (path.uavIsVisuallyEnabledForRerouting)
            {
                condition = EnabledForReroutingOnly;
            }
            
            _uavConditionChangedEventChannel.RaiseEvent(uav, condition);

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
            _uavsGenerator.GenerateUavsRandomly();
        }
        
        
        private void OnUavDestroyed(Uav uav)
        {
            uavs.Remove(uav);
            availableLayers.Add(uav.dedicatedLayer);
        }

        private void OnUavCreated(Uav uav)
        {
            uavs.Add(uav);
        }

        private void GetReferencesFromGameManager()
        {
            _uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            _uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
            _uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
            _uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
            _simulationEndedEventChannel = GameManager.Instance.channelsDatabase.simulationEndedEventChannel;
            _uavSettings = GameManager.Instance.settingsDatabase.uavSettings;
            _navigationSettings = GameManager.Instance.settingsDatabase.navigationSettings;
            _targetDetectionSettings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
            _navigationManager = GameManager.Instance.navigationManager;

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

        public void StartNavigation(float simulationStartTime)
        {
            StartCoroutine( _navigationManager.NavigateAll(simulationStartTime));
        }

        public static int GetEmptyLayer()
        {
            if (availableLayers.Count > 1)
            {
                var layer = availableLayers[0];
                availableLayers.RemoveAt(0);
                return   layer;
            }
            else
            {
                return availableLayers[0]; // last layer, add all uavs to it
            }
        }
    }
}


