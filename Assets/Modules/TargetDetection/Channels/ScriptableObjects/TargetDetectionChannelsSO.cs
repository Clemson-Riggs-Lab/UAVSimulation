using HelperScripts;
using Modules.Navigation.Channels.ScriptableObjects;
using UnityEngine;

namespace Modules.TargetDetection.Channels.ScriptableObjects
{
	[CreateAssetMenu(fileName = "TargetDetectionChannelsSO", menuName = "Channels/TargetDetectionChannels")]

	public class TargetDetectionChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavPathEventChannelSO targetDetectedEventChannel;
		[SerializeField] public UavPathEventChannelSO targetNotDetectedEventChannel;
		
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(targetDetectedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(targetNotDetectedEventChannel,this);
		}
	}
}