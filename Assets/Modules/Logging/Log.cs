using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Modules.Logging
{
	[Serializable]
	public class Log
	{
		public string timestamp=DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff");
		public string simulationTimeStamp= Time.time.ToString();
		public string logType;
		public string eventType;
		public List<string> logMessages;
		
		
	}
}