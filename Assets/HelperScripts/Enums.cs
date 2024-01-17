namespace HelperScripts
{
	public abstract class Enums
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
		public enum ConditionalState
		{
			OFF, // no conditional state
			FP, // On when it shouldn't be
			FN, // Off when it shouldn't be
			TP, // On when it should be
			TN // Off when it should be
			
		}
	}
}