using Events.ScriptableObjects;
using HelperScripts;
using UnityEngine;

namespace Chat.ScriptableObjects
{
	[CreateAssetMenu(fileName = "PromptChannelsSO", menuName = "Channels/Prompt Channels")]

	public class PromptChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		public ObjectEventChannelSO newPromptEventChannel;
		public ObjectEventChannelSO promptResponseReceivedEventChannel;

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(newPromptEventChannel,this);
			AssertionHelper.AssertAssetReferenced(promptResponseReceivedEventChannel,this);
		}
	}
}