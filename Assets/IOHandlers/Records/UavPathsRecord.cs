using System.Collections.Generic;

namespace IOHandlers.Records
{
	public class UavPathsRecord
	{
		public int UavId { get; set; }
		public List<PathRecord> PathRecords { get; set; } = new List<PathRecord>();

		public class PathRecord
		{
			public int DestinationWaypointID { get; set; }
			public bool? UavVisuallyEnabled { get; set; }
			public bool? TargetIsPresent { get; set; }
		}
	}
}