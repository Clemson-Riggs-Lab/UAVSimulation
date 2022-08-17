using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Chat.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Chat Settings", menuName = "Settings/ChatSettings")]
	public class ChatSettingsSO : ScriptableObject
	{
		public enum ChatMessagesSource
		{
			Disabled,
			FromFile,
			FromDefaultRecords,
			RandomMessages,
		}
		
		[Header("ChatBox Settings")]
		public bool enableTextAnimation=true;
		public int textAnimationSpeed=100;
		public int textFontSize = 18;

		public string chatMessagesPrefix = "- [Message from HQ]: ";
		public bool addResponsesToChatBox = true;
		public string responsesPrefix = "---[ Response from Controller]: ";
		

			[Space(20)]
		
		[JsonConverter(typeof(StringEnumConverter))]
		public ChatMessagesSource chatMessagesSource=ChatMessagesSource.FromDefaultRecords;
		
		
		
	
		
		[Space(20)]
		[Header("Responses Settings")]
		public bool shuffleResponses = false;
		public bool giveFeedbackAboutResponsesOnButtons = false;
		public bool giveFeedbackAboutResponsesInChatBox = false;
		public float durationAfterResponseBeforeHidingButtons = 2f;
		public string correctResponseColor = "Green";
		public string wrongResponseColor = "Red";

		
		[Space(20)]
		[Header("Logging Settings")]
		public bool logChatMessages = true;
		public bool logChatMessage = true;
		public bool logAllowedResponseTime = true;
		public bool logCorrectResponseOptions = true;
		public bool logAllResponseOptions = true;
		
		public bool logReceivedResponses = true;
		public bool logCorrectness = true;
		
		public bool logResponses = true;
		//TODO add settings for random messages such as frequency etc.
	}
}