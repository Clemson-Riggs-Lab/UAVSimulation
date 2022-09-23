using Unity.VisualScripting;
using UnityEngine;

namespace UI.FuelAndHealthPanel.Settings.ScriptableObjects
{
   [CreateAssetMenu(fileName = "FuelAndHealthPanelSettings", menuName = "Settings/FuelAndHealthPanelSettings")]
	public class FuelAndHealthPanelSettingsSO: ScriptableObject
	{

      public bool showFuelLeakConditionWhenUavHidden = true;
      
	   [Space(20)] 
      public FuelAndHealthPanelConfigs enabledUavFuelAndHealthPanelConfigs = new()
      {
         healthButtonInteractable = false,
         isVisibile = true,
         healthButtonText = "",
         healthButtonColor = "Green",
         fuelSliderColor = "Green"
      };
      
      [Space(20)] 
      public FuelAndHealthPanelConfigs hiddenUavFuelAndHealthPanelConfigs = new()
      {
         healthButtonInteractable = false,
         isVisibile = false,
         healthButtonText = "",
         healthButtonColor = "Green",
         fuelSliderColor = "Green"
      };

      [Space(20)]
      public FuelAndHealthPanelConfigs fuelLeakFuelAndHealthPanelConfigs= new()
      {
         healthButtonInteractable = true,
         isVisibile=true,
         healthButtonText = "Fix Leak",
         healthButtonTextColor="Black",
         healthButtonColor="Orange",
         fuelSliderColor="Green"
      };
      
      [Space(20)]
      public FuelAndHealthPanelConfigs fatalLeakFuelAndHealthPanelConfigs= new()
       { 
          healthButtonInteractable=false, 
          isVisibile=true,
          healthButtonText = "Fatal Leak",
          healthButtonTextColor="Black",
          healthButtonColor="Red",
          fuelSliderColor="Red"
       };
       
       [Space(20)] 
         public FuelAndHealthPanelConfigs fuelEmptyFuelAndHealthPanelConfigs= new()
         {
            healthButtonInteractable=false,
            isVisibile=true,
            healthButtonText = "Uav Lost",
            healthButtonTextColor="Red",
            healthButtonColor="Black",
            fuelSliderColor="Red"
         };

         [Space(20)] 
         public FuelAndHealthPanelConfigs lostUavFuelAndHealthPanelConfigs= new()
         {
            healthButtonInteractable=false,
            isVisibile=false,
            healthButtonText = "Uav Lost",
            healthButtonTextColor= "Red",
            healthButtonColor="Gray",
            fuelSliderColor="Red"
         };
   }

   [System.Serializable]
   public class FuelAndHealthPanelConfigs
   {
      public bool healthButtonInteractable= false;
      public bool isVisibile= true;
      public string healthButtonText="";
      public string healthButtonTextColor="Black";
      public string healthButtonColor="Green";
      public string fuelSliderColor="Green";
   }
}