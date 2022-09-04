using System;
using Chat.ScriptableObjects;
using Events.ScriptableObjects;
using HelperScripts;
using UnityEngine;

[CreateAssetMenu(fileName = "PromptChannelsSO", menuName = "Database/PromptChannelsSO")]

public class ChannelsDatabase:ScriptableObject
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