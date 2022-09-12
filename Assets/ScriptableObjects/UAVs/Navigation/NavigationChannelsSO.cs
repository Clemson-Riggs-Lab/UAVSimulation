using HelperScripts;
using ScriptableObjects.EventChannels;
using UnityEngine;

namespace ScriptableObjects.UAVs.Navigation
{
	[CreateAssetMenu(fileName = "NavigationChannelsSO", menuName = "Channels/Uav Sub-Modules/Navigation Channels")]

	public class NavigationChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavPathEventChannelSO uavStartedNewPathEventChannel;
		[SerializeField] public UavPathEventChannelSO uavArrivedAtDestinationEventChannel;
		[SerializeField] public UavPathEventChannelSO uavReroutedEventChannel;
		[SerializeField] public UavPathEventChannelSO uavReroutePreviewEventChannel;
		[SerializeField] public UavEventChannelSO reroutingOptionsRequestedChannel;
	

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavStartedNewPathEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavArrivedAtDestinationEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavReroutedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavReroutePreviewEventChannel,this);
			AssertionHelper.AssertAssetReferenced(reroutingOptionsRequestedChannel,this);
			

		}
	}
}