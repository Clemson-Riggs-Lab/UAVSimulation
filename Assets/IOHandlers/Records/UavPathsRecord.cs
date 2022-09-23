using System.Collections.Generic;

namespace IOHandlers
{
	public class UavPathsRecord
	{
		public int UavId { get; set; }
		public List<PathRecord> PathRecords { get; set; } = new List<PathRecord>();

		public class PathRecord
		{
			public int DestinationWayPointID { get; set; }
			public bool? UavVisuallyEnabled { get; set; } = true;
			public bool? TargetIsPresent { get; set; } = false;
		}
	}
}