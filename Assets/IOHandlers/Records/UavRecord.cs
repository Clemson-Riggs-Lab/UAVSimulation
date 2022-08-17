using System.Collections.Generic;

namespace IOHandlers.Records
{
	public class UavRecord
	{
		public int? Id { get; set; }
		public bool? EnabledOnStart { get; set; } = true;
		public int StartingWayPointId { get; set; } = 0;
	}
}