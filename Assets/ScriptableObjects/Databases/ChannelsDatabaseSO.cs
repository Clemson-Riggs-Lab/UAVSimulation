using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.Prompts;
using ScriptableObjects.TargetDetection;
using ScriptableObjects.UAVs;
using ScriptableObjects.UAVs.Navigation;
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

		public TargetDetectionChannelsSO targetDetectionChannels;
		
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
			AssertionHelper.AssertAssetReferenced(targetDetectionChannels,this);
			AssertionHelper.AssertAssetReferenced(wayPointCreatedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(wayPointDisabledEventChannel,this);
			
		
		}
	}
}