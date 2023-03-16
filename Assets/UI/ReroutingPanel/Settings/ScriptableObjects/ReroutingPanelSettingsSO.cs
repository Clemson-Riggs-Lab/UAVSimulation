using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

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
	    public bool hideButtonsForFinishedUavs=true;
	    public bool hideButtonsForLostUavs=true;
	    
	    public bool disableButtonsForHiddenUavs=true;
	    public bool disableButtonsForFinishedUavs=true;
	    public bool disableButtonsForLostUavs=true;
	    
	    [Space(20)]
	    public bool closePanelsForHiddenUavs=true;
	    public bool closePanelsForFinishedUavs=true;
	    public bool closePanelsForLostUavs=true;
	    
	    [Space(20)]
	    public int numberOfReroutingOptionsToPresent = 3;
	    public int numberOfBadReroutingOptionsToPresent = 1;
	    public bool selectShortestPathsAsReroutingOptions=true;
	    
	    public int maximumNumberOfReroutingOptionsPanels = 4;
	    public int numberOfReroutingOptionsPanelsGridRows = 2;
	    public int numberOfReroutingOptionsPanelsGridColumns=2; 
	    [JsonConverter(typeof(StringEnumConverter))]
	    public NewPanelPosition newPanelPosition=NewPanelPosition.PlaceAtTheBeginning;


	    public bool ButtonsColorLikeUav=true;
    }
}
