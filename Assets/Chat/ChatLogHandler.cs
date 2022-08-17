using System;
using System.Collections.Generic;
using System.Linq;
using Chat.ScriptableObjects;
using Events.ScriptableObjects;
using Logging;
using UnityEngine;

namespace Chat
{
	public class ChatLogHandler:MonoBehaviour
	{
		[SerializeField] private ObjectEventChannelSO chatMessageSentChannel;
		[SerializeField] private ObjectEventChannelSO responseReceivedChannel;
		[SerializeField] private LogEventChannelSO loggingChannel;
		
		private ChatSettingsSO _chatSettings;
		
		private void Start()
		{
			_chatSettings= SettingsManager.Instance.chatSettings;
			loggingChannel= LoggingManager.Instance.loggingChannel;
			if(chatMessageSentChannel!=null && _chatSettings.logChatMessages) chatMessageSentChannel.Subscribe(OnChatMessageSent);
			if(responseReceivedChannel!=null && _chatSettings.logReceivedResponses) responseReceivedChannel.Subscribe(OnResponseReceived);
		}

		private void OnResponseReceived(object rp)
		{	if (rp is not ResponseOption response)
				return;
			Log log = new Log
			{
				timestamp = DateTime.Now,
				simulationTimeStamp = Time.time.ToString(),
				logType = "Chat",
				eventType = "ResponseReceived"
			};
			log.logMessages = ComposeResponseLogString(response);
			loggingChannel.RaiseEvent(log);
		}

		

		private void OnChatMessageSent(object chatMsg)
		{
			if (chatMsg is not ChatMessage chatMessage)
				return;
			Log log = new Log
			{
				timestamp = DateTime.Now,
				simulationTimeStamp = Time.time.ToString(),
				logType = "Chat",
				eventType = "ChatMessageSent"
			};
			log.logMessages  = ComposeMessageLogString(chatMessage);
			loggingChannel.RaiseEvent(log);
		}

		private List<string> ComposeMessageLogString(ChatMessage chatMessage)
		{
			var logMessages = new List<string>();
			if (_chatSettings.logChatMessages)
			{
				logMessages.Add("Chat Message: " + chatMessage.consoleMessage);
			}

			if (_chatSettings.logAllowedResponseTime)
			{
				logMessages.Add("Allowed Response Time: " + chatMessage.durationToAcceptResponses.ToString("0.0"));
			}

			if (chatMessage.responseOptions != null)
			{
				if (_chatSettings.logCorrectResponseOptions || _chatSettings.logAllResponseOptions)
				{
					
					var correctResponsesString = "";
					correctResponsesString+= "Correct Response Options: ";
					foreach (var response in chatMessage.responseOptions.Where(response => response.isCorrectResponse))
					{
						correctResponsesString += " | ";
						correctResponsesString += response.buttonText;
					}
					logMessages.Add(correctResponsesString);

				}

				if (_chatSettings.logAllResponseOptions)
				{
					var wrongResponsesString = "";
					wrongResponsesString+= "Wrong Response Options: ";
					foreach (var response in chatMessage.responseOptions.Where(response => !response.isCorrectResponse))
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

			if (_chatSettings.logCorrectness)
			{
				logMessages.Add("Is Correct?: " + response.isCorrectResponse.ToString());
			}
			return logMessages;
		}

		private void OnDisable()
		{
			if(chatMessageSentChannel!=null) chatMessageSentChannel.Unsubscribe(OnChatMessageSent);
			if(responseReceivedChannel!=null) responseReceivedChannel.Unsubscribe(OnResponseReceived);
		}
	}

}
