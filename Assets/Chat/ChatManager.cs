using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chat.ScriptableObjects;
using Events.ScriptableObjects;
using IOHandlers.Records;
using Menu;
using UnityEngine;
using static Chat.ScriptableObjects.ChatSettingsSO.ChatMessagesSource;

namespace Chat
{
	public class ChatManager:MonoBehaviour
	{
		private ChatSettingsSO _chatSettings;

		[SerializeField] private ConsoleTextEventChannelSO writeTextToConsoleChannel;
		[SerializeField] private ObjectEventChannelSO chatMessageSentChannel;
		[SerializeField] private ObjectEventChannelSO responseReceivedChannel;
		
		private List<ChatMessage> _chatMessages = new ();

		public ChatMessage CurrentChatMessage { get; private set; }
		void Start()
		{
			GameManager.Instance.chatManager = this; //telling the game manager that I am the chat manager
			InitializeSettings();
			

			if (responseReceivedChannel != null)
			{
				responseReceivedChannel.Subscribe(OnResponseReceived);
			}

		}

		private void InitializeSettings()
		{
			_chatSettings = SettingsManager.Instance.chatSettings;
		}

		public void Initialize()
		{
			if (_chatSettings.chatMessagesSource != Disabled)
			{
				LoadChatMessages();
			}
			else
			{
				Destroy(this);
			}
			StartCoroutine(StartChatMessagesTimerCoroutine());
		}

		private IEnumerator StartChatMessagesTimerCoroutine()
		{
			foreach (var chatMessage in _chatMessages)
			{
				var deltaTime = chatMessage.timeToPresent - Time.time;
				if (deltaTime > 0)
				{
					yield return new WaitForSeconds(deltaTime);
				}
				CurrentChatMessage= chatMessage;
				SendChatMessage(CurrentChatMessage);
				PopulateResponseOptions(CurrentChatMessage);
				Debug.Log("Chat message was sent after it's time");
			}
		}

		private void PopulateResponseOptions(ChatMessage msg)
		{
			chatMessageSentChannel.RaiseEvent(msg);
		}

		private void OnResponseReceived(object obj)
		{
			//throw new NotImplementedException();
		}

		private void SendChatMessage(ChatMessage msg)
		{
			writeTextToConsoleChannel.RaiseEvent(_chatSettings.chatMessagesPrefix, msg.consoleMessage);
		}
		
		private void LoadChatMessages()
		{
			switch (_chatSettings.chatMessagesSource)
			{
				case FromDefaultRecords:
					LoadChatMessagesFromFile();
					break;
				case FromFile:
					throw new NotImplementedException();
				case RandomMessages:
					throw new NotImplementedException();
				case Disabled:
				default:
					Debug.LogWarning("Chat messages source is not set");
					break;
			}
		}
		
		private void LoadChatMessagesFromFile()
		{
			var chatMessageRecords = DefaultRecordsCreator.GetDefaultChatMessages();//TODO change to dynamically get records from external serialized file.
			if (chatMessageRecords == null)
			{
				Debug.LogWarning("No Chat Messages found in the file");
				return;
			}
			else
			{
				_chatMessages= chatMessageRecords;
			}
		}

		
		private void OnDisable()
		{
			if (responseReceivedChannel != null)
			{
				responseReceivedChannel.Unsubscribe(OnResponseReceived);
			}
		}
	}
}