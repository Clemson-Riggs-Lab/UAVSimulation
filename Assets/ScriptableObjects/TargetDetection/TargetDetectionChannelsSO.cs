using HelperScripts;
using ScriptableObjects.UAVs.Navigation;
using UnityEngine;

namespace ScriptableObjects.TargetDetection
{
	[CreateAssetMenu(fileName = "TargetDetectionChannelsSO", menuName = "Channels/TargetDetectionChannels")]

	public class TargetDetectionChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavPathEventChannelSO targetDetectedButtonClickedEventChannel;
		[SerializeField] public UavPathEventChannelSO targetNotDetectedButtonClickedEventChannel;
		
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(targetDetectedButtonClickedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(targetNotDetectedButtonClickedEventChannel,this);
		}
	}
}