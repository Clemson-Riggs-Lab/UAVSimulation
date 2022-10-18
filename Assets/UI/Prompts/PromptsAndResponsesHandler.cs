using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using Modules.Prompts.Channels.ScriptableObjects;
using Modules.Prompts.Settings.ScriptableObjects;
using Multiplayer;
using TMPro;
using UI.Console.Channels.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Modules.Prompts
{
	public class PromptsAndResponsesHandler : MonoBehaviour
	{
		[NonSerialized] private Dictionary<ResponseOption, GameObject> _responseOptionsButtonDictionary = new();

		[NonSerialized] private PromptEventChannelSO newPromptEventChannel;
		[NonSerialized] private ResponseOptionEventChannelSO responseReceivedChannel;
		[NonSerialized] private ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

		[SerializeField] private Transform responsesContainer;
		[SerializeField] private GameObject responseButtonPrefab;

		[NonSerialized] private PromptSettingsSO _promptSettings;
		[NonSerialized] private IEnumerator _removeButtonsTimerCoroutine;
		[NonSerialized] private bool _allowMultipleResponses;
		[NonSerialized] private float _durationToAcceptResponses;

		private List<ResponseOption> _responseOptions = new List<ResponseOption>();

		private void Start()
		{
			GetSettingsFromGameManager();

			if (newPromptEventChannel != null)
				newPromptEventChannel.Subscribe(OnNewPromptReceivedEvent);

			if (AppNetPortal.Instance.IsMultiplayerMode())
				GameplayNetworkCallsHandler.Instance.ChatResponseClicked_NetworkEventHandler += OnChatResponseClickedNetworkEventHandler;
		}

		private void OnDestroy()
		{
			if (AppNetPortal.Instance.IsMultiplayerMode())
				GameplayNetworkCallsHandler.Instance.ChatResponseClicked_NetworkEventHandler -= OnChatResponseClickedNetworkEventHandler;
		}

		private void GetSettingsFromGameManager()
		{
			_promptSettings = GameManager.Instance.settingsDatabase.promptSettings;
			responseButtonPrefab = GameManager.Instance.prefabsDatabase.responseButtonPrefab;
			newPromptEventChannel = GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
			responseReceivedChannel = GameManager.Instance.channelsDatabase.promptChannels.promptResponseReceivedEventChannel;
			writeMessageToConsoleChannel = GameManager.Instance.channelsDatabase.newConsoleMessageChannel;
		}

		private void OnNewPromptReceivedEvent(Prompt prompt)
		{
			//WriteMessage to console
			writeMessageToConsoleChannel.RaiseEvent(_promptSettings.promptsPrefix, prompt.consoleMessage);

			//Update responses and buttons
			ClearResponsesAndButtons();

			var responseOptions = prompt.responseOptions;
			if (responseOptions == null)
				return;

			_allowMultipleResponses = prompt.acceptMultipleResponses;

			if (_promptSettings.shuffleResponses)
				responseOptions = responseOptions.OrderBy(x => Random.value).ToList();

			AddNewResponseOptions(responseOptions);

			_responseOptions = responseOptions;

			_durationToAcceptResponses = prompt.durationToAcceptResponses;
			if (_durationToAcceptResponses > 0) //not set to default (i.e., there is a limit to how long the player can accept responses)
			{
				_removeButtonsTimerCoroutine = RemoveButtonsTimer(_durationToAcceptResponses);
				StartCoroutine(_removeButtonsTimerCoroutine);
			}
		}

		private void ClearResponsesAndButtons()
		{
			if (_removeButtonsTimerCoroutine != null) //if we have a removeButtonsTimerCoroutine, stop it
				StopCoroutine(_removeButtonsTimerCoroutine);

			_responseOptions.Clear();
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
				var textMp = responseButton.GetComponentInChildren<TextMeshProUGUI>();
				textMp.text = response.buttonText;
				textMp.color = ColorHelper.StringToColor(response.textColor);
				responseButton.name = "Option " + iterator.ToString();
				_responseOptionsButtonDictionary.Add(response, responseButton);
			}
		}

		private void OnResponseButtonClicked(ResponseOption response)
		{
			if (AppNetPortal.Instance.IsMultiplayerMode())
			{
				GameplayNetworkCallsHandler.Instance.ChatReponseClickedServerRpc(response.buttonText);
			}
			else
			{
				ResponseButtonClickedBehaviour(response);
			}
		}

		private void ResponseButtonClickedBehaviour(ResponseOption response)
		{
			if (responseReceivedChannel != null)
				responseReceivedChannel.RaiseEvent(response);

			var button = _responseOptionsButtonDictionary.GetValueOrDefault(response);
			button.GetComponent<Button>().interactable = false;

			if (_promptSettings.giveFeedbackAboutResponsesOnButtons)
			{
				button.GetComponent<Image>().color = response.isCorrectResponse switch
				{
					true => ColorHelper.StringToColor(_promptSettings.correctResponseColor),
					false => ColorHelper.StringToColor(_promptSettings.wrongResponseColor)
				};
			}
			if (_promptSettings.addResponsesToChatBox)
			{
				var message = new ConsoleMessage
				{
					text = response.buttonText,
				};
				if (_promptSettings.giveFeedbackAboutResponsesInChatBox)
					message.color = response.isCorrectResponse switch
					{
						true => _promptSettings.correctResponseColor,
						false => _promptSettings.wrongResponseColor
					};
				writeMessageToConsoleChannel.RaiseEvent(_promptSettings.promptResponsesPrefix, message);
			}

			_responseOptionsButtonDictionary.Remove(response);
			button.GetComponent<Button>().interactable = false;

			if (!_allowMultipleResponses)
			{
				DisableAllButtons();//prevent further clicking
				if (_promptSettings.durationAfterResponseBeforeHidingButtons > 0)
				{
					_removeButtonsTimerCoroutine = RemoveButtonsTimer(_promptSettings.durationAfterResponseBeforeHidingButtons);
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

        private void OnChatResponseClickedNetworkEventHandler(object sender, GameplayNetworkCallsData.NetworkString buttonText)
        {
			ResponseOption responseOption = _responseOptions.Find(x => x.buttonText.Equals(buttonText));

			ResponseButtonClickedBehaviour(responseOption);
        }
    }
}