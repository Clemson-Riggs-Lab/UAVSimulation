using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chat.ScriptableObjects;
using Events.ScriptableObjects;
using HelperScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chat
{
	public class ResponsesHandler : MonoBehaviour
	{
		[SerializeField] private Dictionary<ResponseOption, GameObject> _responseOptionsButtonDictionary = new();

		[SerializeField] private ObjectEventChannelSO chatMessageSentChannel;
		[SerializeField] private ObjectEventChannelSO responseReceivedChannel;
		[SerializeField] private ConsoleTextEventChannelSO sendTextToConsoleChannel;

		[SerializeField] private Transform responsesContainer;
		[SerializeField] private GameObject responseButtonPrefab;

		private ChatSettingsSO _chatSettings;
		private IEnumerator _removeButtonsTimerCoroutine;
		private bool _allowMultipleResponses;
		private float _durationToAcceptResponses;

		private void Start()
		{

			if (chatMessageSentChannel != null)
				chatMessageSentChannel.OnEventRaised += OnPopulateOptionsEventReceived;

			InitializeSettings();
		}

		private void InitializeSettings()
		{
			_chatSettings = SettingsManager.Instance.chatSettings;
			responseButtonPrefab = PrefabsManager.Instance.responseButtonPrefab;
		}

		private void OnPopulateOptionsEventReceived(object chatMsg)
		{
			ClearResponsesAndButtons();

			if (chatMsg is not ChatMessage chatMessage)
				return;
			var responses = chatMessage.responseOptions;
			if (responses == null)
				return;
			_allowMultipleResponses = chatMessage.acceptMultipleResponses;


			if (_chatSettings.shuffleResponses)
				responses = responses.OrderBy(x => Random.value).ToList();


			AddNewResponses(responses);

			_durationToAcceptResponses = chatMessage.durationToAcceptResponses;
			if (_durationToAcceptResponses > 0)
			{
				_removeButtonsTimerCoroutine = RemoveButtonsTimer(_durationToAcceptResponses);
				StartCoroutine(_removeButtonsTimerCoroutine);
			}

			
		}

	

		private void ClearResponsesAndButtons()
		{
			if(_removeButtonsTimerCoroutine != null)
				StopCoroutine(_removeButtonsTimerCoroutine);
			
			_responseOptionsButtonDictionary.Clear();
			foreach (Transform child in responsesContainer)
			{
				Destroy(child.gameObject);
			}
		}

		private void AddNewResponses(List<ResponseOption> responses)
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
			
			if (_chatSettings.giveFeedbackAboutResponsesOnButtons)
			{
				button.GetComponent<Image>().color = response.isCorrectResponse switch
				{
					true => ColorHelper.StringToColor(_chatSettings.correctResponseColor),
					false => ColorHelper.StringToColor(_chatSettings.wrongResponseColor)
				};
			}
			if (_chatSettings.addResponsesToChatBox)
			{
				var message = new ConsoleMessage
				{
					text =response.buttonText,
				};
				if(_chatSettings.giveFeedbackAboutResponsesInChatBox)
					message.color = response.isCorrectResponse switch
					{
						true => _chatSettings.correctResponseColor,
						false => _chatSettings.wrongResponseColor
					};
				sendTextToConsoleChannel.RaiseEvent(_chatSettings.responsesPrefix,message);
				
			}

			_responseOptionsButtonDictionary.Remove(response);
			button.GetComponent<Button>().interactable = false;

			if (!_allowMultipleResponses)
			{
				DisableAllButtons();//prevent further clicking
				_removeButtonsTimerCoroutine = RemoveButtonsTimer(_chatSettings.durationAfterResponseBeforeHidingButtons);
				StartCoroutine(_removeButtonsTimerCoroutine);
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