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
				logMessages = ComposeMessageLogString(chatMessage)
			};
			_logEventChannel.RaiseEvent(log);
		}
		
		private void OnResponseReceived(ResponseOption responseOption)
		{	
			var log = new Log
			{
				logType = "Prompts",
				eventType = "ResponseReceived",
				logMessages = ComposeResponseLogString(responseOption)
			};
			_logEventChannel.RaiseEvent(log);
		}
		

		private List<string> ComposeMessageLogString(Prompt prompt)
		{
			var logMessages = new List<string>();
			if (_promptSettings.logPrompts)
			{
				logMessages.Add("Prompt Message: " + prompt.consoleMessage.text);
			}

			if (_promptSettings.logAllowedResponseTime)
			{
				logMessages.Add("Allowed Response Time: " + prompt.durationToAcceptResponses.ToString("0.0"));
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
					logMessages.Add(correctResponsesString);

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
					logMessages.Add(wrongResponsesString);
				}
			}

			return logMessages;
		}
		
		private List<string> ComposeResponseLogString(ResponseOption response)
		{
			var logMessages = new List<string>();
			logMessages.Add("Response: " + response.buttonText);

			if (_promptSettings.logCorrectness)
			{
				logMessages.Add("Is Correct?: " + response.isCorrectResponse.ToString());
			}
			return logMessages;
		}

		private void OnDisable()
		{
			if(_newPromptEventChannel!=null) _newPromptEventChannel.Unsubscribe(OnPromptMessageSent);
			if(_promptResponseReceivedEventChannel!=null) _promptResponseReceivedEventChannel.Unsubscribe(OnResponseReceived);
		}
	}

}
