using System.Collections.Generic;
using Newtonsoft.Json;

namespace IOHandlers
{ 
	//TODO: Complete this class, then add the functionalities to make the layout dynamic based on a settings input file
	// We want to do this so that adding modules and removing them wouldn't require coding and a recompile of the program.
	// Instead, we can just add the module to the list of modules and then add it in the layout settings in it's right place.
	// However, this is tedious and time consuming currently, so I will delay this until later.
	
	public class LayoutSettingsRecord
	{

		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range TargetSpawnBufferRange { get; set; } = new Range(){ Min = 0, Max = 0 };

		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range VariationInCameraBlankDurations { get; set; } = new Range() { Min = 0, Max = 0 };
		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public bool EnableFuelLeaks { get; set; } = false; //TODO implement in respective class
		
	}

	public class PanelRecord
	{
		public  enum LayoutGroup
		{
			Horizontal,
			Vertical,
			Grid,
			FlexibleGrid
		}
		public string PanelName { get; set; }
		public string PanelColor { get; set; }
		public bool IsLayoutElement { get; set; }
		public bool HasLayoutGroup { get; set; }
		public LayoutGroup LayoutGroupType { get; set; }
		
		public List<PanelRecord> SubPanels { get; set; } = new List<PanelRecord>();
	}
	
	
}