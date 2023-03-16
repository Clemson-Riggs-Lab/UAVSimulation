namespace HelperScripts
{
	public class Enums
	{
		public enum InputRecordsSource 
		{
			FromInputFile, FromDefaultRecords, Dynamic
		}
		
		public enum UavCondition
		{
			EnabledForReroutingOnly,
			EnabledForTargetDetectionOnly,
			EnabledForTargetDetectionAndRerouting,
			Hidden,
			Lost
		}
	}
}