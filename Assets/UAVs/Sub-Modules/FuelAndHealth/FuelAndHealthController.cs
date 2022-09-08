using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.FuelAndHealth;
using ScriptableObjects.UAVs.Navigation;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO.FuelComputationType;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO.FuelConditions;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO.FuelLeaksTypes;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO.UavHealthConditions;

namespace UAVs.Sub_Modules.Fuel
{
    public class FuelAndHealthController : MonoBehaviour
    {
        [NonSerialized]public Uav uav;
        private FuelAndHealthSettingsSO _fuelAndHealthSettings;
        private UavHealthConditions uavHealthCondition= Healthy;
        private FuelConditions _fuelCondition = Normal;
        
        private float _startingFuelLevel;
        private float _fuelLevel;
        private float _fuelConsumptionPerSecond;
        private List<float> _fuelLeakTimes = new();
        
        private bool _simulationActive =false;
        
        // these events are used to communicate with the UI only
        public event Action FuelLevelChanged;
        public event Action FuelConditionChanged;
        public event Action HealthConditionChanged;

        //these Channels are used to communicate with the other modules

        public UavEventChannelSO uavLostEventChannel;
        public UavEventChannelSO uavHealthyEventChannel;
        
        public UavEventChannelSO uavFuelEmptyEventChannel;
        public UavEventChannelSO uavFuelLeakingEventChannel;
        public UavEventChannelSO uavFuelLeakFixedEventChannel;
        public UavEventChannelSO uavFatalFuelLeakEventChannel;
        
        private Uav_FuelConditionEventChannelSO uavFuelConditionChangedEventChannel;
        private Uav_UavHealthConditionEventChannelSO uavHealthConditionChangedEventChannel;
        
        
        public bool IsConsumingFuel { get; set; } = true;

        public float GetStartingFuelLevel() => _startingFuelLevel;

        public void SetStartingFuelLevel(float value)
        {
            _startingFuelLevel = value;
            //also change the current fuel level to match the starting fuel level
            SetFuelLevel(value);
        }

        public float GetFuelLevel() => _fuelLevel;

        public void SetFuelLevel(float value)
        {
            if (value <= 0)
            {
                _fuelLevel = 0;
                SetFuelCondition(Empty);
            }

            _fuelLevel = value;
            FuelLevelChanged?.Invoke();
        }

        public UavHealthConditions GetUavHealthCondition() => uavHealthCondition;

        public void SetUavHealthCondition(UavHealthConditions value)
        {
            uavHealthCondition = value;
            RaiseHealthConditionChanged();
        }


        public FuelConditions GetFuelCondition() => _fuelCondition;

        public void SetFuelCondition(FuelConditions value)
        {
            _fuelCondition = value;
            RaiseFuelConditionChanged();
        }


        private float GetFuelConsumptionRate()
        {
            var multiplier = GetFuelCondition() switch
            {
                Normal => 1,
                Empty => 0,
                Leaking => _fuelAndHealthSettings.fuelConsumptionMultiplierOnLeak,
                FatalLeak => _fuelAndHealthSettings.fuelConsumptionMultiplierOnFatalLeak,
                _ => 0
            };
            return _fuelConsumptionPerSecond * multiplier;
        }

        private void RaiseHealthConditionChanged()
        {
            HealthConditionChanged?.Invoke();
            
            if(uavHealthConditionChangedEventChannel!=null)
                uavHealthConditionChangedEventChannel.RaiseEvent(uav, uavHealthCondition);
            
            switch (GetUavHealthCondition())
            {
                case Healthy:
                    if (uavHealthyEventChannel != null) uavHealthyEventChannel.RaiseEvent(uav);
                    break;
                case Lost:
                    if (uavLostEventChannel != null) uavLostEventChannel.RaiseEvent(uav);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }
     
        private void RaiseFuelConditionChanged()
        {
            FuelConditionChanged?.Invoke();
            
            if(uavFuelConditionChangedEventChannel!=null)
                uavFuelConditionChangedEventChannel.RaiseEvent(uav, GetFuelCondition());
            
            switch (GetFuelCondition())
            {
                case Empty:
                    if (uavFuelEmptyEventChannel != null) uavFuelEmptyEventChannel.RaiseEvent(uav);
                    break;
                case FatalLeak:
                    if (uavFatalFuelLeakEventChannel != null) uavFatalFuelLeakEventChannel.RaiseEvent(uav);
                    break;
                case Leaking:
                    if (uavFuelLeakingEventChannel != null) uavFuelLeakingEventChannel.RaiseEvent(uav);
                    break;
                case Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Awake()
        {
            gameObject.TryGetComponent(out uav);
            if (uav == null)
            {
                Debug.Log("Game object was destroyed because the attached fuel manager requires an attached UAV script",
                    gameObject);
                Destroy(gameObject); // a fuel manager should only be attached to a game object that has a uav script
            }
            _fuelAndHealthSettings = GameManager.Instance.settingsDatabase.uavSettings.fuelAndHealthSettings;
        }

        private void OnEnable()
        {
            InitializeChannels();
        }

        private void InitializeChannels()
        {
            uavLostEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavLostEventChannel;
            uavHealthyEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavHealthyEventChannel;
            uavFuelEmptyEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavFuelEmptyEventChannel;
            uavFuelLeakingEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavFuelLeakingEventChannel;
            uavFuelLeakFixedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavFuelLeakFixedEventChannel;
            uavFatalFuelLeakEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavFatalFuelLeakEventChannel;
            
            uavFuelConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavFuelConditionChangedEventChannel;
            uavHealthConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavHealthConditionChangedEventChannel;
            
        }

        public void Initialize()
        {
            InitializeSettings();

            if (_fuelAndHealthSettings.fuelLeaksType != Disabled)
            {
                PopulateFuelLeaks();
            }
            
        }

        private void InitializeSettings()
        {
            switch (_fuelAndHealthSettings.fuelComputationType)
            {
                case Duration:
                    SetStartingFuelLevel(_fuelAndHealthSettings.fuelDuration);
                    _fuelConsumptionPerSecond = 1f;
                    break;
                case Consumption:
                    SetStartingFuelLevel(_fuelAndHealthSettings.fuelCapacity);
                    _fuelConsumptionPerSecond = _fuelAndHealthSettings.fuelConsumptionPerSecond;
                    break;

                default:
                    CalculateFuelLevelAndConsumptionBasedOnPathsDurations();
                    break;
                
            }

            SetUavHealthCondition(_fuelAndHealthSettings.startingUavUavHealthCondition);
        }
        
        public void Begin()
        {
            _simulationActive = true;
            // coroutine that updates the fuel level every second
            StartCoroutine(UpdateFuelLevel());
        }

        private IEnumerator UpdateFuelLevel()
        {
            while ( IsConsumingFuel)
            {
                if (_simulationActive)
                    SetFuelLevel(GetFuelLevel() - GetFuelConsumptionRate());
               
                if (GetFuelLevel() <= 0)
                    IsConsumingFuel = false;
                
                yield return new WaitForSeconds(1f);
            }
        }


        private void PopulateFuelLeaks()
        {
            
            switch (_fuelAndHealthSettings.fuelLeaksType)
            {
                
                case FromFile:
                {
                    //select using linq where uav id = uav id
                    var fuelLeakRecord = GameManager.Instance.jsonSerializerTest.rootObject.FuelLeaksRecord.FirstOrDefault(x => x.UavID == uav.ID);//TODO move records to their own SO
                    if (fuelLeakRecord == null)
                    {
                        Debug.LogWarning("No fuel leak record found for UAV with ID: " + uav.ID);
                        return;
                    }
                    else
                    {
                        _fuelLeakTimes = fuelLeakRecord.FuelLeakTimes.OrderBy(x => x).ToList();
                        StartCoroutine(StartFuelLeaksTimerCoroutine());
                    }

                    break;
                }
                case RandomLeaks:
                    throw new NotImplementedException(); //TODO
                
                case Disabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private IEnumerator StartFuelLeaksTimerCoroutine()
        {
            foreach (var fuelLeakTime in _fuelLeakTimes)
            {
                var deltaTime = fuelLeakTime - Time.time;
                if (deltaTime > 0)
                {
                    yield return new WaitForSeconds(deltaTime); 
                    SetFuelCondition(Leaking);
                    Debug.Log("Fuel leak at time: " + fuelLeakTime);
                    yield return new WaitForSeconds(_fuelAndHealthSettings.fuelLeakDuration);
                                                                              
                    if(GetFuelCondition() == Normal)
                        continue;
                    else
                    {
                        SetFuelCondition(FatalLeak);
                    }
                }
                else
                {
                    Debug.Log("A fuel leak timing error occured. Either one fuel leak is placed within the duration of another one, or the fuel leak times are not in order. Regardless, this fuel Leak is ignored.");
                }
               
            }
        }
        
        
        private void CalculateFuelLevelAndConsumptionBasedOnPathsDurations()
        {
            var navigator = uav.GetComponent<Navigator>();
            if (navigator == null)
            {
                Debug.LogWarning("No navigator found on UAV with ID: " + uav.ID+". couldn't compute fuel starting level based on paths");
                return;
            }
            else
            {
                _fuelConsumptionPerSecond = 1f;
                if (navigator.speedMode == NavigationSettingsSO.SpeedMode.FixedPathDuration)
                {
                    SetStartingFuelLevel(navigator.Paths.Count * navigator.pathDuration);
                            
                }
                else
                {
                    SetStartingFuelLevel(navigator.Paths.Count * 20);
                    Debug.LogWarning("couldn't calculate fuel level based on paths duration since navigation speed mode is set to fixed speed. assuming 20 seconds per path");
                }
            }
        }

        public void OnHealthButtonClicked(string buttonText)
        {
            if (buttonText == _fuelAndHealthSettings.fuelLeakFuelAndHealthPanelSettings.fuelAndHealthPanelVisualSettings.healthButtonText &&  GetFuelCondition()== Leaking)
            {
                SetFuelCondition(Normal);
                
                if (uavFuelLeakFixedEventChannel != null)
                {
                    uavFuelLeakFixedEventChannel.RaiseEvent(uav);
                }
            }
            else
            {
                Debug.LogError("Unknown error: either button text is wrong or fuel was not leaking when the button was pressed. In both cases, this shouldn't have happened " + buttonText);
            }
            
        }
        
    }
}