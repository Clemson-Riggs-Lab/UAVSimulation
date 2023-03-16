using System.Collections.Generic;

namespace IOHandlers
{
	public class UavDynamicNavigationWorkloadRecord
	{
		public float TimeOfChange { get; set; }
		public float FrequencyOfHeadToNFZ {get; set; }
		public float RatioOfVisibleUavsForRerouting {get; set; }
	}
}