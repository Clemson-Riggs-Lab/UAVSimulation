using Events.ScriptableObjects;
using HelperScripts;
using UAVs.Sub_Modules.Navigation.ScriptableObjects;
using UnityEngine;

namespace UAVs.Sub_Modules.Navigation.Channels
{
	[CreateAssetMenu(fileName = "NavigationChannelsSO", menuName = "Channels/Uav Sub-Modules/Navigation Channels")]

	public class NavigationChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavPathEventChannelSO uavStartedNewPathEventChannel;
		[SerializeField] public UavPathEventChannelSO uavArrivedAtDestinationEventChannel;
		[SerializeField] public UavPathEventChannelSO uavReroutedEventChannel;
	

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavStartedNewPathEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavArrivedAtDestinationEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavReroutedEventChannel,this);

		}
	}
}