using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ScriptableObjects.UAVs.Navigation.Rerouting
{
	[CreateAssetMenu(fileName = "ReroutingSettings", menuName = "Settings/ReroutingSettings")]
    public class ReroutingSettingsSO:ScriptableObject
    {

	    public enum NewPanelPosition
	    {
		    PlaceAtTheBeginning ,PlaceAtTheEnd
	    }



	    [Space(20)]
	    public int numberOfReroutingOptionsToPresent = 3;
	    public int numberOfBadReroutingOptionsToPresent = 1;
	    public bool selectShortestPathsAsReroutingOptions=true;
	    
	    public int maximumNumberOfReroutingOptionsPanels = 4;
	    public int numberOfReroutingOptionsPanelsGridRows = 2;
	    public int numberOfReroutingOptionsPanelsGridColumns=2; 
	    [JsonConverter(typeof(StringEnumConverter))]
	    public NewPanelPosition newPanelPosition=NewPanelPosition.PlaceAtTheBeginning;
	   
	 
    }
}
