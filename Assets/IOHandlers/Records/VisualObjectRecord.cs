using JetBrains.Annotations;

namespace IOHandlers.Records
{
	public class VisualObjectRecord
	{
		public int? ID { get; set; } 
		public bool? IsTarget { get; set; }
		[CanBeNull] public string ObjectType { get; set;}
		public int? WaypointID { get; set; }
		public Coordinates PositionRelativeToWaypoint { get; set; } = new Coordinates(); // if Y is null/not set, then the object is placed on the ground
		
	}
}