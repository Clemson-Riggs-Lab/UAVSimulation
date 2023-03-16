using System.Collections.Generic;

namespace IOHandlers
{
	public class UavDynamicTargetDetectionWorkloadRecord
	{
		public float TimeOfChange { get; set; }
		public float RatioOfFeedsWithTarget {get; set; }
		public float RatioOfFeedsWithNonTarget {get; set; }
		public float RatioOfVisibleUavsForTargetDetection {get; set; }

	}
}