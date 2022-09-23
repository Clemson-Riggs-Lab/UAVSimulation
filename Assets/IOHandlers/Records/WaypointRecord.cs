namespace IOHandlers
{
	public class WayPointRecord
	{
		public int? Id { get; set; }
		public Coordinates Position { get; set; } = new Coordinates();
	}
}