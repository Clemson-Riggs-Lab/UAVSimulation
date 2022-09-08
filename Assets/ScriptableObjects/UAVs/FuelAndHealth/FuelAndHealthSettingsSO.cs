using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static ScriptableObjects.UAVs.Camera.UavCameraSettingsSO;
using static ScriptableObjects.UAVs.Camera.UavCameraSettingsSO.UavConditions;
using static ScriptableObjects.UAVs.Camera.UavCameraSettingsSO.UavVideoArtifacts;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO.FuelStatusAndHealthBarPositioning;

namespace ScriptableObjects.UAVs.FuelAndHealth
{
   [CreateAssetMenu(fileName = "FuelAndHealthSettings", menuName = "Settings/FuelAndHealthSettings")]
   public class FuelAndHealthSettingsSO : ScriptableObject
   {
     
      
      [Header("Health Settings")]
      [JsonConverter(typeof(StringEnumConverter))]
      public UavHealthConditions startingUavUavHealthCondition = UavHealthConditions.Healthy;
      

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
      public FuelLeaksTypes fuelLeaksType= FuelLeaksTypes.FromFile;
      public float fuelConsumptionMultiplierOnLeak=5;
      public float fuelLeakDuration=10;
      public float fuelLeakButtonInteractionDurationBeforeFatalLeak=10f;  
      public float fuelConsumptionMultiplierOnFatalLeak=20;

      [Space(20)]
   
      public FuelStatusAndHealthBarPositioning fuelStatusAndHealthBarPositioning = FuelStatusAndHealthBarVisibleInSeparatePanel;

      [Space(20)] 
      public FuelAndHealthPanelSettings healthyUavFuelAndHealthPanelSettings = new()
      {
         healthButtonInteractable = false,
         uavCondition = Flying,
         videoArtifacts = None,
         
         fuelAndHealthPanelVisualSettings = new()
         {
            healthButtonText = "",
            healthButtonColor = "Green",
            fuelSliderColor = "Green"
         },
      };

      [Space(20)]
      public FuelAndHealthPanelSettings fuelLeakFuelAndHealthPanelSettings= new()
      {
         healthButtonInteractable = true,
         uavCondition=Flying,
         videoArtifacts=None,
         
         fuelAndHealthPanelVisualSettings = new()
         {
         healthButtonText = "Fix Leak",
         healthButtonTextColor="Black",
         healthButtonColor="Orange",
         fuelSliderColor="Green"
      },
        
      };
      
      [Space(20)]
      public FuelAndHealthPanelSettings fatalLeakFuelAndHealthPanelSettings= new()
       { 
          healthButtonInteractable=false, 
          uavCondition=Descending,
          videoArtifacts=None,
          
          fuelAndHealthPanelVisualSettings = new()
         {
            healthButtonText = "Fatal Leak",
            healthButtonTextColor="Black",
            healthButtonColor="Red",
            fuelSliderColor="Red"
         },
       };
       
       [Space(20)] 
         public FuelAndHealthPanelSettings fuelEmptyFuelAndHealthPanelSettings= new()
         {
            healthButtonInteractable=false,
            uavCondition=FallAndCrash,
            videoArtifacts=BlackScreen,
            
            fuelAndHealthPanelVisualSettings = new()
            {
               healthButtonText = "Uav Lost",
               healthButtonTextColor="Red",
               healthButtonColor="Black",
               fuelSliderColor="Red"
            },
          
         };

         [Space(20)] 
         public FuelAndHealthPanelSettings uavLostFuelAndHealthPanelSettings= new()
         {
            healthButtonInteractable=false,
            uavCondition= UavConditions.Disabled,
            videoArtifacts=BlackScreen,
            
            fuelAndHealthPanelVisualSettings = new()
            {
               
               healthButtonText = "Uav Lost",
               healthButtonTextColor= "Red",
               healthButtonColor="Gray",
               fuelSliderColor="Red"
            },

               
         };
         
         
         
         public enum FuelComputationType
         {
            Duration,
            Consumption
         }

         public enum FuelLeaksTypes
         {
            Disabled,
            RandomLeaks,
            FromFile
         }
         public enum FuelStatusAndHealthBarPositioning
         {
            FuelStatusAndHealthBarVisibleInSeparatePanel,
            FuelStatusOnlyVisibleInSeparatePanel,
            HealthBarOnlyVisibleInSeparatePanel,
            FuelStatusAndHealthBarVisibleInCameraWindow,
            FuelStatusOnlyVisibleInCameraWindow,
            HealthBarOnlyVisibleInCameraWindow
         }
         public enum UavHealthConditions
         {
            Healthy,
            Lost
         }
         public enum FuelConditions
         {
            Normal,
            Leaking,
            FatalLeak,
            Empty
         }

   }

   [System.Serializable]
   public class FuelAndHealthPanelSettings
   {
      public bool healthButtonInteractable= false;
      [JsonConverter(typeof(StringEnumConverter))]
      public UavConditions uavCondition= Flying;
      [JsonConverter(typeof(StringEnumConverter))]
      public UavVideoArtifacts videoArtifacts= None;
      public FuelAndHealthPanelVisualSettings fuelAndHealthPanelVisualSettings;
   }
   
   [System.Serializable]
   public class FuelAndHealthPanelVisualSettings
   {
      public string healthButtonText="";
      public string healthButtonTextColor="Black";
      public string healthButtonColor="Green";
      public string fuelSliderColor="Green";
   }
   
  
}
