using System;
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
		private DateTime _startTime; 
		
		
		private void Start()
		{
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			
			if(_logEventChannel!=null)
			{
				_logEventChannel.Subscribe(OnLogReceived);
			}
			_startTime = DateTime.Now;
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
			var logFolder = Application.dataPath + "/logFiles/";
			if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
			
			string json = JsonConvert.SerializeObject(Logs, Formatting.Indented);
			var participantNumber= GameManager.Instance.participantNumber;
			var trialNumber = GameManager.Instance.trialNumber;
			using StreamWriter file = File.CreateText(logFolder+ "p"+ participantNumber + "_t" + trialNumber + "_" + _startTime.ToString("yyyy-MM-dd HH-mm-ss") +".json");
			file.Write(json);
		}
	}
}