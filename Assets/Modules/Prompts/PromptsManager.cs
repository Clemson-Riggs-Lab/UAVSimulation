using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using IOHandlers;
using Modules.Prompts.Channels.ScriptableObjects;
using Modules.Prompts.Settings.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums.InputRecordsSource;

namespace Modules.Prompts
{
	public class PromptsManager:MonoBehaviour
	{
		private PromptSettingsSO _promptSettings;
		private PromptEventChannelSO _newPromptEventChannel;
		private List<Prompt> _prompts = new (); 
		public Prompt CurrentPrompt { get; private set; }
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			LoadPrompts();
			
			var promptsLogHandler = gameObject.GetOrAddComponent<PromptLogHandler>();
			promptsLogHandler.Initialize();
		}

		private void GetReferencesFromGameManager()
		{
			_promptSettings = GameManager.Instance.settingsDatabase.promptSettings;
			_newPromptEventChannel= GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
		}
		
		public IEnumerator StartPromptsTimerCoroutine(float simulationStartTime)
		{
			if (_prompts.Count == 0)
			{
				Debug.LogError("No prompts found");
				yield break;
			}
			
			yield return new WaitForSeconds(simulationStartTime-Time.time);
			
			foreach (var prompt in _prompts)
			{
				var deltaTime = prompt.timeToPresent +GameManager.Instance.simulationStartTime - Time.time;
				if (deltaTime > 0)
				{
					yield return new WaitForSeconds(deltaTime);
				}
				else if (deltaTime < -0.1) // if the prompt is very late 
				{
					Debug.Log("Prompt was sent after it's time");
				}
				CurrentPrompt= prompt;
				SendPrompt(CurrentPrompt);
				
			}
		}

		private void SendPrompt(Prompt msg)
		{
			_newPromptEventChannel.RaiseEvent(msg);
		}
		
		
		public void LoadPrompts()
		{
			_prompts = _promptSettings.promptsSource switch
			{
				FromInputFile => GameManager.Instance.inputRecordsDatabase.Prompts,
				FromDefaultRecords => DefaultRecordsCreator.AddDefaultPromptRecords(),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}