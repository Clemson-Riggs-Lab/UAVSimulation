using System.Collections.Generic;

namespace IOHandlers.Records
{
	public class UavRecord
	{
		public int? Id { get; set; }
		public bool? EnabledOnStart { get; set; } = true;
		public int StartingWaypointId { get; set; } = 0;
	}
}