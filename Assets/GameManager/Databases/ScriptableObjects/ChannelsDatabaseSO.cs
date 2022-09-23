using HelperScripts;
using Modules.FuelAndHealth.Channels.ScriptableObjects;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Prompts.Channels.ScriptableObjects;
using Modules.TargetDetection.Channels.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UAVs.Channels.ScriptableObjects;
using UI.Console.Channels.ScriptableObjects;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;

namespace Databases.ScriptableObjects
{
	[CreateAssetMenu(fileName = "PromptChannelsSO", menuName = "Database/PromptChannelsSO")]

	public class ChannelsDatabaseSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		public ConsoleMessageEventChannelSO newConsoleMessageChannel;
		public VoidEventChannelSO simulationEndedEventChannel;
		public LogEventChannelSO logEventChannel;
		public PromptChannelsSO promptChannels;
		public UavChannelsSO uavChannels;
		
		 public NavigationChannelsSO navigationChannels;
		 public FuelChannelsSO fuelChannels;
		 
		public TargetDetectionChannelsSO targetDetectionChannels;
		
		public WaypointEventChannelSO wayPointCreatedEventChannel;
		public WaypointEventChannelSO wayPointDisabledEventChannel;
	
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(newConsoleMessageChannel,this);
			AssertionHelper.AssertAssetReferenced(simulationEndedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(logEventChannel,this);
			AssertionHelper.AssertAssetReferenced(promptChannels,this);
			AssertionHelper.AssertAssetReferenced(uavChannels,this);
			AssertionHelper.AssertAssetReferenced(navigationChannels,this);
			AssertionHelper.AssertAssetReferenced(fuelChannels,this);
			AssertionHelper.AssertAssetReferenced(targetDetectionChannels,this);
			AssertionHelper.AssertAssetReferenced(wayPointCreatedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(wayPointDisabledEventChannel,this);
			
		
		}
	}
}