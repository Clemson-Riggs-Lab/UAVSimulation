using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.ReroutingPanel.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "ReroutingPanelSettings", menuName = "Settings/ReroutingPanelSettings")]
    public class ReroutingPanelSettingsSO:ScriptableObject
    {

	    public enum NewPanelPosition
	    {
		    PlaceAtTheBeginning ,PlaceAtTheEnd
	    }
	    

	    [Space(20)]
	    public bool hideButtonsForHiddenUavs=true;
	    public bool keepHiddenButtonsPositions=true;
	    public bool hideButtonsForLostUavs=true;
	    
	    public bool disableButtonsForHiddenUavs=true;
	    public bool disableButtonsForLostUavs=true;
	    
	    [Space(20)]
	    public bool closePanelsForHiddenUavs=true;
	    public bool closePanelsForLostUavs=true;
	    
	    [Space(20)]
	    public int numberOfReroutingOptionsToPresent = 3;
	    public int numberOfBadReroutingOptionsToPresent = 1;
	    public int maximumNumberOfReroutingOptionsPanels = 4;
	    public int numberOfReroutingOptionsPanelsGridRows = 2;
	    public int numberOfReroutingOptionsPanelsGridColumns=2; 
	    public int shuffleReroutingOptionsRandomGeneratorSeed = 1;

	    [JsonConverter(typeof(StringEnumConverter))]
	    public NewPanelPosition newPanelPosition=NewPanelPosition.PlaceAtTheBeginning;
	    
	    [Space(20)]
	    public string headerText = "Primary task";
	    public string headerTextColor = "light green";
	    public bool onlyAccountForVisibleUavsButtonsInLayout = false;
	    public bool colorButtonsLikeUav=true;
    }
}
