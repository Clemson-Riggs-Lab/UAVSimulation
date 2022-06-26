using System.Collections.Generic;

namespace IOHandlers.Records
{
	public class UavFuelLeaksRecord
	{
		public int UavID { get; set; }
		public List<float> FuelLeakTimes { get; set; }
	}
}