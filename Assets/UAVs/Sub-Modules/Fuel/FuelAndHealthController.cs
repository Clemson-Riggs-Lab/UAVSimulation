using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UAVs.Navigation;
using UAVs.Navigation.ScriptableObjects;
using UAVs.Sub_Modules.Fuel.ScriptableObjects;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using static UAVs.Sub_Modules.Fuel.ScriptableObjects.FuelAndHealthSettingsSO;
using static UAVs.Sub_Modules.Fuel.ScriptableObjects.FuelAndHealthSettingsSO.FuelComputationType;
using static UAVs.Sub_Modules.Fuel.ScriptableObjects.FuelAndHealthSettingsSO.FuelConditions;
using static UAVs.Sub_Modules.Fuel.ScriptableObjects.FuelAndHealthSettingsSO.FuelLeaksTypes;
using static UAVs.Sub_Modules.Fuel.ScriptableObjects.FuelAndHealthSettingsSO.HealthConditions;

namespace UAVs.Sub_Modules.Fuel
{
    public class FuelAndHealthController : MonoBehaviour
    {
        [NonSerialized]public Uav uav;
        public int uavId;
        private FuelAndHealthSettingsSO _fuelAndHealthSettings;
        private HealthConditions _healthCondition= Healthy;
        private FuelConditions _fuelCondition = Normal;
        
        private float _startingFuelLevel;
        private float _fuelLevel;
        private float _fuelConsumptionPerSecond;
        private List<float> _fuelLeakTimes = new();
        
        private bool _simulationActive =false;
        

        public bool IsConsumingFuel { get; set; } = true;
        public float StartingFuelLevel
        {
            get => _startingFuelLevel;
            set
            { 
                _startingFuelLevel = value;
                //also change the current fuel level to match the starting fuel level
                FuelLevel = value;
            }
        }

        public float FuelLevel
        {
            get => _fuelLevel;
            set
            {
                if (value <= 0)
                {
                    _fuelLevel= 0;
                    fuelCondition = Empty;
                }
                _fuelLevel = value;
                FuelLevelChanged?.Invoke();
            }
        }
        public HealthConditions HealthCondition
        {
            get => _healthCondition;
            set
            {
                _healthCondition = value;
                HealthConditionChanged?.Invoke();
            }
        }

        public FuelConditions fuelCondition
        {
            get => _fuelCondition;
            set
            {
                _fuelCondition = value;
                FuelConditionChanged?.Invoke();

            }
        }
        
        private float FuelConsumptionRate
        {
            get
            {
                var multiplier = fuelCondition switch
                {
                    Normal => 1,
                    Empty => 0,
                    Leaking => _fuelAndHealthSettings.fuelConsumptionMultiplierOnLeak,
                    FatalLeak => _fuelAndHealthSettings.fuelConsumptionMultiplierOnFatalLeak,
                    _ => 0
                };
                return _fuelConsumptionPerSecond * multiplier;
            }
        }


        public event Action FuelLevelChanged;
        public event Action FuelConditionChanged;
        public event Action HealthConditionChanged;
       
        
        private void Awake()
        {
            gameObject.TryGetComponent(out uav);
            if (uav == null)
            {
                Debug.Log("Game object was destroyed because the attached fuel manager requires an attached UAV script",
                    gameObject);
                Destroy(gameObject); // a fuel manager should only be attached to a game object that has a uav script
            }

            uavId = uav.ID;
            _fuelAndHealthSettings = GameManager.Instance.settingsDatabase.uavSettings.fuelAndHealthSettings;
        }

        private void Update()
        {
            if(!_simulationActive || !IsConsumingFuel)
                return;
            FuelLevel -= FuelConsumptionRate * Time.deltaTime;
            
            
        }

       

        public void Initialize()
        {
            InitializeVariables();

            if (_fuelAndHealthSettings.fuelLeaksType != Disabled)
            {
                PopulateFuelLeaks();
            }
            
        }

        private void InitializeVariables()
        {
            switch (_fuelAndHealthSettings.fuelComputationType)
            {
                case Duration:
                    StartingFuelLevel=_fuelAndHealthSettings.fuelDuration;
                    _fuelConsumptionPerSecond = 1f;
                    break;
                case Consumption:
                    StartingFuelLevel = _fuelAndHealthSettings.fuelCapacity;
                    _fuelConsumptionPerSecond = _fuelAndHealthSettings.fuelConsumptionPerSecond;
                    break;

                default:
                    CalculateFuelLevelAndConsumptionBasedOnPathsDurations();
                    break;
                
            }

            HealthCondition = _fuelAndHealthSettings.healthCondition;
        }
        
        public void Begin()
        {
            _simulationActive = true;
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
                    fuelCondition = Leaking;
                    Debug.Log("Fuel leak at time: " + fuelLeakTime);
                    yield return new WaitForSeconds(_fuelAndHealthSettings.fuelLeakDuration);
                                                                              
                    if(fuelCondition == Normal)
                        continue;
                    else
                    {
                        fuelCondition = FatalLeak;
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
                    StartingFuelLevel = navigator.Paths.Count * navigator.pathDuration;
                            
                }
                else
                {
                    StartingFuelLevel = navigator.Paths.Count * 20;
                    Debug.LogWarning("couldn't calculate fuel level based on paths duration since navigation speed mode is set to fixed speed. assuming 20 second per path");
                }
            }
        }

        public void OnHealthButtonClicked(string buttonText)
        {
            if (buttonText == _fuelAndHealthSettings.fuelLeakPanelSettings.panelVisualSettings.healthButtonText &&  fuelCondition== Leaking)
            {
                fuelCondition = Normal;
            }
            else
            {
                Debug.LogError("Unknown error: either button text is wrong or fuel was not leaking when the button was pressed. In both cases, this shouldn't have happened " + buttonText);
            }
            
        }
    }
}