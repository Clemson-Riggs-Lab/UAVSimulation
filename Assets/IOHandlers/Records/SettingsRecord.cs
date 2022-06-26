using Newtonsoft.Json;

namespace IOHandlers.Records
{
	public class SettingsRecord
	{
		public float TimePeriodDurationInSecs { get; set; } = 10;
		public string SceneName { get; set; } = "GridWaypoints";
		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range TargetSpawnBufferRange { get; set; } = new Range(){ Min = 0, Max = 0 };
		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public bool BlankCamerasBetweenNavigations { get; set; } = false; //TODO implement in camera handler

		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range VariationInCameraBlankDurations { get; set; } = new Range() { Min = 0, Max = 0 };
		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public bool EnableFuelLeaks { get; set; } = false; //TODO implement in respective class
		
	}
}