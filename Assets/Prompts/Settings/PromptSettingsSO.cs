using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.InputRecordsSource;

namespace Prompts.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Chat Settings", menuName = "Settings/ChatSettings")]
	public class PromptSettingsSO : ScriptableObject
	{

		[Space(20)]
		[JsonConverter(typeof(StringEnumConverter))]
		public InputRecordsSource promptsSource=FromInputFile;
		
		[Space(20)]
		[Header("ChatBox Settings")]
		public bool enableTextAnimation=true;
		public int textAnimationSpeed=100;
		public int textFontSize = 18;

		[Space(20)]
		[Header("Prompt Settings")]
		public string promptsPrefix = "- [Message from HQ]: ";
		public bool addResponsesToChatBox = true;
		public string promptResponsesPrefix = "---[ Response from Controller]: ";


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
		public bool logPrompts = true;
		public bool logAllowedResponseTime = true;
		public bool logCorrectResponseOptions = true;
		public bool logAllPromptResponseOptions = true;
		public bool logReceivedResponses = true;
		public bool logCorrectness = true;
		
		//TODO add settings for random messages such as frequency etc.
	}
}