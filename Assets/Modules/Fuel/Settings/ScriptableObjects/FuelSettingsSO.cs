using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.InputRecordsSource;

namespace Modules.FuelAndHealth.Settings.ScriptableObjects
{
   [CreateAssetMenu(fileName = "FuelSettings", menuName = "Settings/FuelSettings")]
   public class FuelSettingsSO : ScriptableObject
   {
     
      
      [Header("Fuel Consumption calculation method: Based on duration or based on consumption per second")]
      [JsonConverter(typeof(StringEnumConverter))]
      public FuelComputationType fuelComputationType;
      public float fuelDuration=950f;//in seconds
      [Tooltip("Not used in Duration mode")]
      public float fuelCapacity=950f;//in gallons
      [Tooltip("Not used in Duration mode")]
      public float fuelConsumptionPerSecond=1f;//in gallons per second
      [Space(20)]

      [Header("Fuel Leak Settings")]
      [JsonConverter(typeof(StringEnumConverter))]
      public InputRecordsSource fuelLeaksRecordsSource= FromInputFile;
      public float fuelConsumptionMultiplierOnLeak=5;
      public float fuelLeakDuration=10;
      public float fuelLeakButtonInteractionDurationBeforeFatalLeak=10f;  
      public float fuelConsumptionMultiplierOnFatalLeak=20;

      
      [Space(20)]
      [Header("Fuel Logging Settings")]
      public bool logFuelLeakEvents;
      public bool logFuelLeakFixEvents;
      public bool logFatalFuelLeakEvents;
      public bool logFuelEmptyEvents;
      
         
         
         public enum FuelComputationType
         {
            Duration,
            Consumption
         }
         
         
         public enum FuelCondition
         {
            Normal,
            Leaking,
            FatalLeak,
            Empty
         }

        
   }

  
   
  
}
