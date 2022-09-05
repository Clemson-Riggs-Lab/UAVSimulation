using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ScriptableObjects.EventChannels;
using UnityEngine;

namespace Logging
{
	public class LoggingManager:MonoBehaviour
	{		
		public static LoggingManager Instance { get; private set; }
		private static List<Log> Logs { get; set; } = new ();
		private LogEventChannelSO logEventChannel;

		
		
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
			logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			
			if(logEventChannel!=null)
			{
				logEventChannel.Subscribe(OnLogReceived);
			}
			
			InitializeLogHandlers();

		}

		private void InitializeLogHandlers()
		{
			if (true) // todo get these from configs e.g.
			{
				var promptLogHandler = gameObject.AddComponent<PromptLogHandler>();
				promptLogHandler.Initialize();
			}

			if (true) //other logger
			{
				
			}
			//.... 
		}

		private void OnLogReceived(Log log)
		{
			Logs.Add(log);
		}

		private void OnDisable()
		{
			if(logEventChannel!=null)
			{
				logEventChannel.Unsubscribe(OnLogReceived);
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