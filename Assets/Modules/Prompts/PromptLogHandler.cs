using System.Collections.Generic;
using System.Linq;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Prompts.Channels.ScriptableObjects;
using Modules.Prompts.Settings.ScriptableObjects;
using UnityEngine;

namespace Modules.Prompts
{
	public class PromptLogHandler:MonoBehaviour
	{
		private PromptEventChannelSO _newPromptEventChannel;
		private ResponseOptionEventChannelSO _promptResponseReceivedEventChannel;
		private LogEventChannelSO _logEventChannel;
		
		private PromptSettingsSO _promptSettings;
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}
		
		private void GetReferencesFromGameManager()
		{
			_promptSettings= GameManager.Instance.settingsDatabase.promptSettings;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_newPromptEventChannel= GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
			_promptResponseReceivedEventChannel= GameManager.Instance.channelsDatabase.promptChannels.promptResponseReceivedEventChannel;
		}
		
		private void SubscribeToChannels()
		{
			if(_newPromptEventChannel!=null && _promptSettings.logPrompts==true) _newPromptEventChannel.Subscribe(OnPromptMessageSent);
			if(_promptResponseReceivedEventChannel!=null && _promptSettings.logReceivedResponses==true) _promptResponseReceivedEventChannel.Subscribe(OnResponseReceived);
		}
		
		private void OnPromptMessageSent(Prompt chatMessage)
		{
			var log = new Log
			{
				logType = "Prompts",
				eventType = "PromptMessageSent",
				logData = ComposeMessageLogString(chatMessage)
			};
			_logEventChannel.RaiseEvent(log);
		}
		
		private void OnResponseReceived(ResponseOption responseOption)
		{	
			var log = new Log
			{
				logType = "Prompts",
				eventType = "ResponseReceived",
				logData = ComposeResponseLogString(responseOption)
			};
			_logEventChannel.RaiseEvent(log);
		}
		

		private List<string> ComposeMessageLogString(Prompt prompt)
		{
			var logData = new List<string>();
			if (_promptSettings.logPrompts)
			{
				logData.Add("Prompt Message: " + prompt.consoleMessage.text);
			}

			if (_promptSettings.logAllowedResponseTime)
			{
				logData.Add("Allowed Response Time: " + prompt.durationToAcceptResponses.ToString("0.0"));
			}

			if (prompt.responseOptions != null)
			{
				if (_promptSettings.logCorrectResponseOptions || _promptSettings.logAllPromptResponseOptions)
				{
					
					var correctResponsesString = "";
					correctResponsesString+= "Correct Response Options: ";
					foreach (var response in prompt.responseOptions.Where(response => response.isCorrectResponse))
					{
						correctResponsesString += " | ";
						correctResponsesString += response.buttonText;
					}
					logData.Add(correctResponsesString);

				}

				if (_promptSettings.logAllPromptResponseOptions)
				{
					var wrongResponsesString = "";
					wrongResponsesString+= "Wrong Response Options: ";
					foreach (var response in prompt.responseOptions.Where(response => !response.isCorrectResponse))
					{
						wrongResponsesString += " | ";
						wrongResponsesString += response.buttonText;
					}
					logData.Add(wrongResponsesString);
				}
			}

			return logData;
		}
		
		private List<string> ComposeResponseLogString(ResponseOption response)
		{
			var logData = new List<string>();
			logData.Add("Response: " + response.buttonText);

			if (_promptSettings.logCorrectness)
			{
				logData.Add("Is Correct?: " + response.isCorrectResponse.ToString());
			}
			return logData;
		}

		private void OnDisable()
		{
			if(_newPromptEventChannel!=null) _newPromptEventChannel.Unsubscribe(OnPromptMessageSent);
			if(_promptResponseReceivedEventChannel!=null) _promptResponseReceivedEventChannel.Unsubscribe(OnResponseReceived);
		}
	}

}
