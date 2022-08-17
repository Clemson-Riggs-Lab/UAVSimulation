using System;
using System.Collections.Generic;
using System.IO;
using Events.ScriptableObjects;
using Newtonsoft.Json;
using UnityEngine;

namespace Logging
{
	public class LoggingManager:MonoBehaviour
	{		
		public static LoggingManager Instance { get; private set; }
		private static List<Log> Logs { get; set; }
		[SerializeField] public LogEventChannelSO loggingChannel;

		
		
		private void Awake() 
		{ 
			// If there is an instance, and it's not me, delete myself.
    
			if (Instance != null && Instance != this) 
			{ 
				Destroy(this); 
			} 
			else 
			{ 
				Instance = this; 
			} 
		}
		
		
		private void Start()
		{

			if(loggingChannel!=null)
			{
				loggingChannel.Subscribe(OnLogReceived);
			}

			Logs = new();
		}

		private void OnLogReceived(Log log)
		{
			Logs.Add(log);
		}

		private void OnDisable()
		{
			if(loggingChannel!=null)
			{
				loggingChannel.Unsubscribe(OnLogReceived);
			}

			WriteLogsToFile();
			
			

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