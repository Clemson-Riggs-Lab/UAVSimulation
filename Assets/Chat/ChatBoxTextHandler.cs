using System;
using System.Collections;
using HelperScripts;
using Menu;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Chat
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Chat.ScriptableObjects;
	using Events.ScriptableObjects;
	using HelperScripts;
	using TMPro;
	using Unity.VisualScripting;
	using UnityEditor;
	using UnityEngine;
	using static HelperScripts.NatoAlphabetConverter;

	namespace Menu
	{
		public class ChatBoxTextHandler : MonoBehaviour
		{
			
			[SerializeField] private TextMeshProUGUI consoleTextMeshProUGUI;
			[SerializeField] public TypeWriterEffect typeWriterEffectScript;
			[DoNotSerialize] private QueueManager _animationQueue;

			[SerializeField] private ConsoleTextEventChannelSO writeTextToConsoleChannel;
			

			private ChatSettingsSO _chatSettings;
			private bool _doAnimateByDefault = false; // change to false to disable animation by default
			

			private void OnValidate()
			{
				MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(consoleTextMeshProUGUI, this, this.gameObject);
				MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(typeWriterEffectScript, this, this.gameObject);
			}

			private void Awake()
			{

				if (writeTextToConsoleChannel != null)
					writeTextToConsoleChannel.OnEventRaised += OnConsoleTextReceived;
				
			}

			

			private void Start()
			{
				_animationQueue= gameObject.AddComponent<QueueManager>();
				InitializeSettingsFromSO();
			}
			private void InitializeSettingsFromSO()
			{
				_chatSettings = SettingsManager.Instance.chatSettings;
				_doAnimateByDefault = _chatSettings.enableTextAnimation;
				consoleTextMeshProUGUI.fontSize = _chatSettings.textFontSize;
				typeWriterEffectScript.durationOfSingleCharacterAnimation=1f/_chatSettings.textAnimationSpeed;
			}

			private void OnConsoleTextReceived(string prefix, ConsoleMessage message)
			{
				message.text = prefix + message.text;
				AddTextToConsole(message);
			}
			
			private void AddTextToConsole(ConsoleMessage message)
			{
				var textToAdd = Environment.NewLine + message.text;
				if(message.color != null)
					textToAdd=  TextManipulation.AddColorToText(textToAdd, message.color);
				
				var animate= _doAnimateByDefault;
				//if message DoAnimate is not null, then use it to override the default animation choice
				if(message.DoAnimate != null)
					animate = message.DoAnimate.Value;
				_animationQueue.AddToQueue(animate ? typeWriterEffectScript.AddAnimatedText(textToAdd) : AddTextWithoutAnimation(textToAdd));
			}
			
			private IEnumerator AddTextWithoutAnimation(string textToAdd)
			{
				consoleTextMeshProUGUI.text += textToAdd;
				consoleTextMeshProUGUI.maxVisibleCharacters = consoleTextMeshProUGUI.textInfo.characterCount;
				yield return null;
			}
			
			
			private void OnDisable()
			{
				if (writeTextToConsoleChannel != null)
					writeTextToConsoleChannel.OnEventRaised -= OnConsoleTextReceived;
			}
		}
	}
}