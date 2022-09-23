using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IOHandlers;
using Modules.FuelAndHealth.Channels.ScriptableObjects;
using Modules.FuelAndHealth.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO.FuelComputationType;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO.FuelCondition;

namespace Modules.FuelAndHealth
{
    public class FuelController : MonoBehaviour
    {
        [NonSerialized] private Uav _uav;
        private FuelSettingsSO _fuelSettings;
        private FuelCondition _fuelCondition = Normal;
        
        private float _fuelLevel;
        private float _fuelConsumptionPerSecond;
        private List<float> _fuelLeakTimes = new();
        
        //these Channels are used to communicate with the other modules
        public UavEventChannelSO uavFuelLeakFixedEventChannel;
        public UavFloatEventChannelSO uavFuelLevelChangedEventChannel;
        private UavFuelConditionEventChannelSO _uavFuelConditionChangedEventChannel;
        private UavConditionEventChannelSO _uavConditionChangedEventChannel;

        public bool isConsumingFuel;

        private FuelCondition FuelCondition
        {
            set
            {
                _fuelCondition = value;
                if(_uavFuelConditionChangedEventChannel!=null)
                    _uavFuelConditionChangedEventChannel.RaiseEvent(_uav, _fuelCondition);
            }
            get => _fuelCondition;
        }
        
        private float FuelConsumptionRate => 
            FuelCondition switch
            {
                Normal => _fuelConsumptionPerSecond,
                Leaking => _fuelConsumptionPerSecond*_fuelSettings.fuelConsumptionMultiplierOnLeak,
                FatalLeak => _fuelConsumptionPerSecond*_fuelSettings.fuelConsumptionMultiplierOnFatalLeak,
                Empty => 0,
                _ => 0
            };

        private void GetReferencesFromGameManager()
        {
            _fuelSettings = GameManager.Instance.settingsDatabase.fuelSettings;
            uavFuelLeakFixedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelLeakFixedEventChannel;
            _uavFuelConditionChangedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelConditionChangedEventChannel;
            _uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
            uavFuelLevelChangedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelLevelChangedEventChannel;
        }

        public void Initialize(Uav uav)
        {
            GetReferencesFromGameManager();
            _uav = uav;
            SetStartingFuelLevelAndConsumptionRate();
            PopulateFuelLeaks();
        }

        private void SetStartingFuelLevelAndConsumptionRate()
        {
            switch (_fuelSettings.fuelComputationType)
            {
                case Duration:
                    _fuelLevel=_fuelSettings.fuelDuration;
                    _fuelConsumptionPerSecond = 1f;
                    break;
                case Consumption:
                    _fuelLevel=_fuelSettings.fuelCapacity;
                    _fuelConsumptionPerSecond = _fuelSettings.fuelConsumptionPerSecond;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Begin()
        {
            isConsumingFuel = true;
            StartCoroutine(UpdateFuelLevel());
            StartCoroutine(StartFuelLeaksTimerCoroutine());
        }

        private IEnumerator UpdateFuelLevel()
        {
            while ( isConsumingFuel)
            {
                _fuelLevel -= FuelConsumptionRate;
                if (_fuelLevel <= 0)
                {
                    isConsumingFuel = false;
                    FuelCondition=Empty;
                    _uavConditionChangedEventChannel.RaiseEvent(_uav, UavCondition.Lost);
                    yield break;
                }
                uavFuelLevelChangedEventChannel.RaiseEvent(_uav, _fuelLevel);
                yield return new WaitForSeconds(1f);
            }
        }
        private void PopulateFuelLeaks()
        {
            var records = _fuelSettings.fuelLeaksRecordsSource switch
            {
                InputRecordsSource.FromInputFile => GameManager.Instance.inputRecordsDatabase.FuelLeaksRecord.FirstOrDefault(x => x.UavID == _uav.id),
                InputRecordsSource.FromDefaultRecords => DefaultRecordsCreator.GetDefaultFuelLeaksRecord().FirstOrDefault(x => x.UavID == _uav.id),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (records != null)
            {
                _fuelLeakTimes = records.FuelLeakTimes.OrderBy(x => x).ToList();
            }
            else
            {
                Debug.LogWarning("No fuel leak records found for UAV with UavName: " + _uav.uavName);
            }
        }

        private IEnumerator StartFuelLeaksTimerCoroutine()
        {
            foreach (var fuelLeakTime in _fuelLeakTimes)
            {
                var deltaTime = fuelLeakTime +GameManager.Instance.simulationStartTime - Time.time;
                if (deltaTime >= 0)
                {
                    if (_uav.uavCondition is Lost or Finished)
                    {
                        Debug.Log("UAV is lost, no fuel leaks will be simulated");
                        yield break;
                    }
                    
                    else
                    {
                        yield return new WaitForSeconds(deltaTime); 
                        FuelCondition=Leaking;
                        Debug.Log("Fuel leak at time: " + fuelLeakTime);
                        yield return new WaitForSeconds(_fuelSettings.fuelLeakDuration);

                        if(FuelCondition != Normal)
                        {
                            FuelCondition=FatalLeak;
                        }
                    }
                   
                }
                else
                {
                    Debug.Log("A fuel leak timing error occured. Either one fuel leak is placed within the duration of another one, or the fuel leak times are not in order. Regardless, this fuel Leak is ignored.");
                }
               
            }
        }
        
        public void OnHealthButtonClicked(string buttonText)
        {
            if (FuelCondition== Leaking)
            {
                FuelCondition=Normal;
                
                if (uavFuelLeakFixedEventChannel != null)
                    uavFuelLeakFixedEventChannel.RaiseEvent(_uav);
            }
            else
            {
                Debug.LogError("Fuel leak cannot be fixed when the UAV is not leaking");
            }
            
        }
        
    }
}