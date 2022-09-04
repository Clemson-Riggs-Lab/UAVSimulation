using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Logging
{
	[Serializable]
	public class Log
	{
		public DateTime timestamp=DateTime.Now;
		public string simulationTimeStamp= Time.time.ToString(CultureInfo.InvariantCulture);
		public string logType;
		public string eventType;
		public List<string> logMessages;
		
		
	}
}