using System.Collections.Generic;
using System.Linq;

namespace IOHandlers
{
	public class UavRecord
	{
		public int? Id { get; set; }
		public bool? EnabledOnStart { get; set; } = true;
		public int StartingWayPointId { get; set; } = 0;
		
		public static List<UavRecord> HandleNullValues(List<UavRecord> uavsRecords)
		{
			var maxId = uavsRecords.Max(x => x.Id) ?? 0;
			foreach (var uavsRecord in uavsRecords)
			{
				uavsRecord.Id ??= maxId + 1;
			}
			return uavsRecords;
		}
	}
	
	
}