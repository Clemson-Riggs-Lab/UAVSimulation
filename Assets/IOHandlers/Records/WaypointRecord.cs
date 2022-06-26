namespace IOHandlers.Records
{
	public class WaypointRecord
	{
		public int? Id { get; set; }
		public Coordinates Position { get; set; } = new Coordinates();
	}
}