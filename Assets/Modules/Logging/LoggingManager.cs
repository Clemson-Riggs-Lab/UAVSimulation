using System.Collections.Generic;
using System.IO;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Prompts;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.Logging
{
	public class LoggingManager:MonoBehaviour
	{		
		private static List<Log> Logs { get; set; } = new ();
		private LogEventChannelSO _logEventChannel;
		
		
		private void Start()
		{
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			
			if(_logEventChannel!=null)
			{
				_logEventChannel.Subscribe(OnLogReceived);
			}
		}
		

		private void OnLogReceived(Log log)
		{
			Logs.Add(log);
		}

		private void OnDisable()
		{
			if(_logEventChannel!=null)
			{
				_logEventChannel.Unsubscribe(OnLogReceived);
			}

			FixLogTimes();
			
			WriteLogsToFile();
		}

		private void FixLogTimes()
		{
			foreach (var log in Logs)
			{
				log.simulationTimeStamp = (float.Parse(log.simulationTimeStamp) + GameManager.Instance.simulationStartTime).ToString();
			}
		}

		private void WriteLogsToFile()
		{
			string json = JsonConvert.SerializeObject(Logs, Formatting.Indented);
			using StreamWriter file = File.CreateText(@"D:\ChatLogs.json");
			JsonSerializer serializer = new JsonSerializer();
			file.Write(json);
		}
	}
}