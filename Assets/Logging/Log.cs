using System;
using System.Collections.Generic;

namespace Logging
{
	[Serializable]
	public class Log
	{
		public DateTime timestamp;
		public string simulationTimeStamp;
		public string logType;
		public string eventType;
		public List<string> logMessages;
		
		
	}
}