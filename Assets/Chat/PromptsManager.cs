using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using IOHandlers.Records;
using Menu;
using Prompts.ScriptableObjects;
using UnityEngine;
using static Prompts.ScriptableObjects.PromptSettingsSO.PromptsSource;

namespace Prompts
{
	public class PromptsManager:MonoBehaviour
	{
		private PromptSettingsSO promptSettings;
		private ObjectEventChannelSO newPromptEventChannel;
		private List<Prompt> prompts = new (); 
		public Prompt CurrentPrompt { get; private set; }
		
		public void Initialize()
		{
			GameManager.Instance.promptsManager = this; //telling the game manager that I am the prompt manager
			GetSettingsFromGameManager();
			
			if (promptSettings.promptsSource != Disabled)
			{
				LoadPrompts();
			}
			else
			{
				Destroy(this);
			}
			StartCoroutine(StartPromptsTimerCoroutine());
		}

		private void GetSettingsFromGameManager()
		{
			promptSettings = GameManager.Instance.settingsDatabase.promptSettings;
			newPromptEventChannel= GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
		}
		
		private IEnumerator StartPromptsTimerCoroutine()
		{
			foreach (var prompt in prompts)
			{
				var deltaTime = prompt.timeToPresent - Time.time;
				if (deltaTime > 0)
				{
					yield return new WaitForSeconds(deltaTime);
				}
				CurrentPrompt= prompt;
				SendPrompt(CurrentPrompt);
				Debug.Log("Prompt was sent after it's time");
			}
		}

		private void SendPrompt(Prompt msg)
		{
			newPromptEventChannel.RaiseEvent(msg);
		}
		
		
		private void LoadPrompts()
		{
			switch (promptSettings.promptsSource)
			{
				case FromDefaultRecords:
					LoadPromptsFromDefaultRecords();
					break;
				case FromFile:
					LoadPromptsFromFile();
					break;
				case RandomMessages:
					throw new NotImplementedException();
				case Disabled:
				default:
					Debug.LogWarning("Prompts source is not set");
					break;
			}
		}
		
		private void LoadPromptsFromDefaultRecords()
		{
			var promptsRecords = DefaultRecordsCreator.GetDefaultPrompts();
			if (promptsRecords == null)
			{
				Debug.LogWarning("No Prompts were found in the file");
				return;
			}
			else
			{
				prompts= promptsRecords;
			}
		}

		private void LoadPromptsFromFile()
		{
			//TODO change to dynamically get records from external serialized file.
			//placeholder code below
			var promptsRecords = DefaultRecordsCreator.GetDefaultPrompts();
			if (promptsRecords == null)
			{
				Debug.LogWarning("No Prompts were found in the file");
				return;
			}
			else
			{
				prompts= promptsRecords;
			}
		}
		
		private void OnDisable()
		{
			
		}
	}
}