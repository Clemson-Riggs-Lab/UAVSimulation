using System;
using System.Collections.Generic;
using System.Linq;
using Prompts;
using Prompts.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UnityEngine;

namespace Logging
{
	public class PromptLogHandler:MonoBehaviour
	{
		private ObjectEventChannelSO newPromptEventChannel;
		private ObjectEventChannelSO promptResponseReceivedEventChannel;
		private LogEventChannelSO logEventChannel;
		
		private PromptSettingsSO promptSettings;
		
		public void Initialize()
		{
			InitializeSettings();
			SubscribeToChannels();
		}
		
		private void InitializeSettings()
		{
			promptSettings= GameManager.Instance.settingsDatabase.promptSettings;
			logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			newPromptEventChannel= GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
			promptResponseReceivedEventChannel= GameManager.Instance.channelsDatabase.promptChannels.promptResponseReceivedEventChannel;
		}
		
		private void SubscribeToChannels()
		{
			if(newPromptEventChannel!=null && promptSettings.logPrompts==true) newPromptEventChannel.Subscribe(OnChatMessageSent);
			if(promptResponseReceivedEventChannel!=null && promptSettings.logReceivedResponses==true) promptResponseReceivedEventChannel.Subscribe(OnResponseReceived);
		}
		
		private void OnChatMessageSent(object chatMsg)
		{
			if (chatMsg is not Prompt chatMessage)
				return;
			var log = new Log
			{
				logType = "Chat",
				eventType = "ChatMessageSent",
				logMessages = ComposeMessageLogString(chatMessage)
			};
			logEventChannel.RaiseEvent(log);
		}
		
		private void OnResponseReceived(object rp)
		{	if (rp is not ResponseOption response)
				return;
			var log = new Log
			{
				logType = "Chat",
				eventType = "ResponseReceived",
				logMessages = ComposeResponseLogString(response)
			};
			logEventChannel.RaiseEvent(log);
		}
		

		private List<string> ComposeMessageLogString(Prompt prompt)
		{
			var logMessages = new List<string>();
			if (promptSettings.logPrompts)
			{
				logMessages.Add("Chat Message: " + prompt.consoleMessage);
			}

			if (promptSettings.logAllowedResponseTime)
			{
				logMessages.Add("Allowed Response Time: " + prompt.durationToAcceptResponses.ToString("0.0"));
			}

			if (prompt.responseOptions != null)
			{
				if (promptSettings.logCorrectResponseOptions || promptSettings.logAllPromptResponseOptions)
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

				if (promptSettings.logAllPromptResponseOptions)
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

			if (promptSettings.logCorrectness)
			{
				logMessages.Add("Is Correct?: " + response.isCorrectResponse.ToString());
			}
			return logMessages;
		}

		private void OnDisable()
		{
			if(newPromptEventChannel!=null) newPromptEventChannel.Unsubscribe(OnChatMessageSent);
			if(promptResponseReceivedEventChannel!=null) promptResponseReceivedEventChannel.Unsubscribe(OnResponseReceived);
		}
	}

}
