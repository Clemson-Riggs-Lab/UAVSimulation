using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers.Records;
using Menu;
using Prompts.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UnityEngine;
using static HelperScripts.Enums.InputRecordsSource;

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
			GetReferencesFromGameManager();
			LoadPrompts();
		}

		private void GetReferencesFromGameManager()
		{
			promptSettings = GameManager.Instance.settingsDatabase.promptSettings;
			newPromptEventChannel= GameManager.Instance.channelsDatabase.promptChannels.newPromptEventChannel;
		}
		
		public IEnumerator StartPromptsTimerCoroutine(float simulationStartTime)
		{
			if (prompts.Count == 0)
			{
				Debug.LogError("No prompts found");
				yield break;
			}
			
			yield return new WaitForSeconds(simulationStartTime-Time.time);
			
			foreach (var prompt in prompts)
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
			newPromptEventChannel.RaiseEvent(msg);
		}
		
		
		public void LoadPrompts()
		{
			prompts = promptSettings.promptsSource switch
			{
				FromInputFile => GameManager.Instance.inputRecordsDatabase.Prompts,
				FromDefaultRecords => DefaultRecordsCreator.AddDefaultPromptRecords(),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
		
		
		private void OnDisable()
		{
			
		}
	}
}