using JetBrains.Annotations;

namespace IOHandlers.Records
{
	public class VisualObjectRecord
	{
		public int? ID { get; set; } 
		public bool? IsTarget { get; set; }
		[CanBeNull] public string ObjectType { get; set;}
		public int? WayPointID { get; set; }
		public Coordinates PositionRelativeToWayPoint { get; set; } = new Coordinates(); // if Y is null/not set, then the object is placed on the ground
		
	}
}