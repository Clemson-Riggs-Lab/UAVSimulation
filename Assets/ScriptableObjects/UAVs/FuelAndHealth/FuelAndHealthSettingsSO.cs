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
      public enum HealthConditions
      {
         Healthy,
         Unavailable,
         Lost
      }
      public enum FuelConditions
      {
         Normal,
         Leaking,
         FatalLeak,
         Empty
      }
      
      [Header("Health Settings")]
      [JsonConverter(typeof(StringEnumConverter))]
      public HealthConditions healthCondition = HealthConditions.Healthy;
      

      [Header("Fuel Consumption calculation method: Based on duration or based on consumption per second")]
      [JsonConverter(typeof(StringEnumConverter))]
      public FuelComputationType fuelComputationType;
      public float fuelDuration=950f;//in seconds
      [Tooltip("Not used in Duration mode")]
      public float fuelCapacity=950f;//in gallons
      [Tooltip("Not used in Duration mode")]
      public float fuelConsumptionPerSecond=1f;//in gallons per second

      [Space(20)]
   
      public FuelStatusAndHealthBarPositioning fuelStatusAndHealthBarPositioning = FuelStatusAndHealthBarVisibleInSeparatePanel;

      [Space(20)] 
      [Header("Healthy UAV Settings")]
      public PanelSettings healthyPanelSettings = new()
      {
         healthButtonInteractable = false,
         uavCondition = Flying,
         videoArtifacts = None,
         
         panelVisualSettings = new()
         {
            healthButtonText = "",
            healthButtonColor = "Green",
            fuelSliderColor = "Green"
         },
      };
         

      public string damagedUavColor="Orange";
      public string deadUavColor="Black";
   
      [Space(20)]

      [Header("Fuel Leak Settings")]
      [JsonConverter(typeof(StringEnumConverter))]
      public FuelLeaksTypes fuelLeaksType= FuelLeaksTypes.FromFile;
      public float fuelConsumptionMultiplierOnLeak=5;
      public float fuelLeakDuration=10;
      public float fuelLeakButtonInteractionDurationBeforeFatalLeak=10f;  
      
      public PanelSettings fuelLeakPanelSettings= new()
      {
         healthButtonInteractable = true,
         uavCondition=Flying,
         videoArtifacts=Shake,
         
         panelVisualSettings = new()
         {
         healthButtonText = "Fix Leak",
         healthButtonTextColor="Black",
         healthButtonColor="Orange",
         fuelSliderColor="Green"
      },
        
      };
      
      
      [Space(20)] 
      [Header("Fatal Leak Settings")]
      public float fuelConsumptionMultiplierOnFatalLeak=20;

       public PanelSettings fatalLeakPanelSettings= new()
       { 
          healthButtonInteractable=false, 
          uavCondition=Descending,
          videoArtifacts=Noise,
          
          panelVisualSettings = new()
         {
            healthButtonText = "Fatal Leak",
            healthButtonTextColor="Black",
            healthButtonColor="Red",
            fuelSliderColor="Red"
         },
       };
       
       [Space(20)] 
       [Header("Fuel Empty Settings")]
         public PanelSettings fuelEmptyPanelSettings= new()
         {
            healthButtonInteractable=false,
            uavCondition=FallAndCrash,
            videoArtifacts=AllNoise,
            
            panelVisualSettings = new()
            {
               healthButtonText = "Uav Lost",
               healthButtonTextColor="Red",
               healthButtonColor="Black",
               fuelSliderColor="Red"
            },
          
         };

         [Space(20)] 
         [Header("Uav Unavailable Settings")]
         public PanelSettings uavUnavailablePanelSettings= new()
         {
            healthButtonInteractable=false,
            uavCondition=UavConditions.Flying,
            videoArtifacts=ConnectionIssues,
            
            panelVisualSettings = new()
            {
               
               healthButtonText = "Uav Unavailable",
               healthButtonTextColor= "White",
               healthButtonColor="Black",
               fuelSliderColor="Red"
            },
          
         };
         
         [Space(20)] 
         [Header("Uav Lost Settings")]
         public PanelSettings uavLostPanelSettings= new()
         {
            healthButtonInteractable=false,
            uavCondition= UavConditions.Disabled,
            videoArtifacts=BlackScreen,
            
            panelVisualSettings = new()
            {
               
               healthButtonText = "Uav Lost",
               healthButtonTextColor= "Red",
               healthButtonColor="Gray",
               fuelSliderColor="Red"
            },

               
         };

   }

   [System.Serializable]
   public class PanelSettings
   {
      public bool healthButtonInteractable= false;
      [JsonConverter(typeof(StringEnumConverter))]
      public UavConditions uavCondition= Flying;
      [JsonConverter(typeof(StringEnumConverter))]
      public UavVideoArtifacts videoArtifacts= None;
      public PanelVisualSettings panelVisualSettings;

   }
   [System.Serializable]
   public class PanelVisualSettings
   {
      public string healthButtonText="";
      public string healthButtonTextColor="Black";
      public string healthButtonColor="Green";
      public string fuelSliderColor="Green";
   }
   
  
}
