using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using HelperScripts;
using Prompts.ScriptableObjects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Prompts
{
	public class PromptsAndResponsesHandler : MonoBehaviour
	{
		[SerializeField] private Dictionary<ResponseOption, GameObject> _responseOptionsButtonDictionary = new();

		[SerializeField] private ObjectEventChannelSO newPromptEventChannel;
		[SerializeField] private ObjectEventChannelSO responseReceivedChannel;
		[SerializeField] private ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

		[SerializeField] private Transform responsesContainer;
		[SerializeField] private GameObject responseButtonPrefab;

		[DoNotSerialize] private PromptSettingsSO promptSettings;
		[DoNotSerialize] private IEnumerator _removeButtonsTimerCoroutine;
		[DoNotSerialize] private bool _allowMultipleResponses;
		[DoNotSerialize] private float _durationToAcceptResponses;

		private void Start()
		{

			if (newPromptEventChannel != null)
				newPromptEventChannel.OnEventRaised += OnNewPromptReceivedEvent;
			
			GetSettingsFromGameManager();
		}

		private void GetSettingsFromGameManager()
		{
			promptSettings = GameManager.Instance.settingsDatabase.promptSettings;
			responseButtonPrefab = GameManager.Instance.prefabsDatabase.responseButtonPrefab;
			newPromptEventChannel = GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
			responseReceivedChannel = GameManager.Instance.channelsDatabase.promptChannels.promptResponseReceivedEventChannel;
		}

		private void OnNewPromptReceivedEvent(object ob)
		{
			if (ob is not Prompt prompt)
				return;
			
			//WriteMessage to console
			writeMessageToConsoleChannel.RaiseEvent(promptSettings.promptsPrefix, prompt.consoleMessage);
			
			//Update responses and buttons
			ClearResponsesAndButtons();

			var responseOptions = prompt.responseOptions;
			if (responseOptions == null)
				return;
			
			_allowMultipleResponses = prompt.acceptMultipleResponses;

			if (promptSettings.shuffleResponses)
				responseOptions = responseOptions.OrderBy(x => Random.value).ToList();

			AddNewResponseOptions(responseOptions);

			_durationToAcceptResponses = prompt.durationToAcceptResponses;
			if (_durationToAcceptResponses > 0) //not set to default (i.e., there is a limit to how long the player can accept responses)
			{
				_removeButtonsTimerCoroutine = RemoveButtonsTimer(_durationToAcceptResponses);
				StartCoroutine(_removeButtonsTimerCoroutine);
			}
		}

	

		private void ClearResponsesAndButtons()
		{
			if(_removeButtonsTimerCoroutine != null) //if we have a removeButtonsTimerCoroutine, stop it
				StopCoroutine(_removeButtonsTimerCoroutine);
			
			_responseOptionsButtonDictionary.Clear();
			foreach (Transform child in responsesContainer)
			{
				Destroy(child.gameObject);
			}
		}

		private void AddNewResponseOptions(List<ResponseOption> responses)
		{
			var iterator = 0;
			foreach (var response in responses)
			{
				iterator++;
				var responseButton = Instantiate(responseButtonPrefab, responsesContainer);
				responseButton.GetComponent<Button>().onClick.AddListener(() => OnResponseButtonClicked(response));
				responseButton.GetComponent<Image>().color = ColorHelper.StringToColor(response.buttonColor);
				var textMP = responseButton.GetComponentInChildren<TextMeshProUGUI>();
				textMP.text = response.buttonText;
				textMP.color = ColorHelper.StringToColor(response.textColor);
				responseButton.name = "Option "+iterator.ToString();
				_responseOptionsButtonDictionary.Add(response, responseButton);
			}
		}

		private void OnResponseButtonClicked(ResponseOption response)
		{
			if (responseReceivedChannel != null)
				responseReceivedChannel.RaiseEvent(response);

			var button = _responseOptionsButtonDictionary.GetValueOrDefault(response);
			button.GetComponent<Button>().interactable = false;
			
			if (promptSettings.giveFeedbackAboutResponsesOnButtons)
			{
				button.GetComponent<Image>().color = response.isCorrectResponse switch
				{
					true => ColorHelper.StringToColor(promptSettings.correctResponseColor),
					false => ColorHelper.StringToColor(promptSettings.wrongResponseColor)
				};
			}
			if (promptSettings.addResponsesToChatBox)
			{
				var message = new ConsoleMessage
				{
					text =response.buttonText,
				};
				if(promptSettings.giveFeedbackAboutResponsesInChatBox)
					message.color = response.isCorrectResponse switch
					{
						true => promptSettings.correctResponseColor,
						false => promptSettings.wrongResponseColor
					};
				writeMessageToConsoleChannel.RaiseEvent(promptSettings.promptResponsesPrefix,message);
				
			}

			_responseOptionsButtonDictionary.Remove(response);
			button.GetComponent<Button>().interactable = false;

			if (!_allowMultipleResponses)
			{
				DisableAllButtons();//prevent further clicking
				if (promptSettings.durationAfterResponseBeforeHidingButtons > 0)
				{
					_removeButtonsTimerCoroutine =
						RemoveButtonsTimer(promptSettings.durationAfterResponseBeforeHidingButtons);
					StartCoroutine(_removeButtonsTimerCoroutine);
				}
			}
			
		}

		private void DisableAllButtons()
		{
			foreach (var button in _responseOptionsButtonDictionary.Values)
			{
				button.GetComponent<Button>().interactable = false;
			}
		}


		private IEnumerator RemoveButtonsTimer(float timer)
		{
			yield return new WaitForSeconds(timer);
			ClearResponsesAndButtons();
		}
		
	}
}