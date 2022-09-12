namespace IOHandlers.Records
{
	public class NFZRecord
	{
		public Coordinates StartCoordinates { get; set; }= new Coordinates();
		public Coordinates EndCoordinates { get; set; }= new Coordinates();
		public string TextOnNFZAfterCountdown { get; set; } = "";
		public float NFZStartTime { get; set; }= 0;
		public float NFZEndTime { get; set; }= 0;
		public float NFZCountdownTimer { get; set; }= 0;

	}
}