namespace IOHandlers
{
	public class Range
	{
		public Range(int min, int max)
		{
			Min = min;
			Max = max;
		}
		public Range()
		{
			Min = 0;
			Max = 0;
		}
		public float Min { get; set; }
		public float Max { get; set; }
	}
}