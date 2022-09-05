using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.Prompts;
using ScriptableObjects.UAVs;
using UnityEngine;

namespace ScriptableObjects.Databases
{
	[CreateAssetMenu(fileName = "PromptChannelsSO", menuName = "Database/PromptChannelsSO")]

	public class ChannelsDatabaseSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		public ConsoleMessageEventChannelSO newConsoleMessageChannel;
		public LogEventChannelSO logEventChannel;
		public PromptChannelsSO promptChannels;
		public UavChannelsSO uavChannels;
	
		public ObjectEventChannelSO wayPointCreatedEventChannel;
		public ObjectEventChannelSO wayPointDisabledEventChannel;
	
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(newConsoleMessageChannel,this);
			AssertionHelper.AssertAssetReferenced(logEventChannel,this);
			AssertionHelper.AssertAssetReferenced(promptChannels,this);
			AssertionHelper.AssertAssetReferenced(uavChannels,this);
		
		}
	}
}