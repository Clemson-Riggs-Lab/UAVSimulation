using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.FuelAndHealth;
using ScriptableObjects.UAVs.Navigation;
using UnityEngine;

namespace ScriptableObjects.UAVs
{
	[CreateAssetMenu(fileName = "UavChannelsSO", menuName = "Channels/Uav Channels")]

	public class UavChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavEventChannelSO uavCreatedEventChannel;
		[SerializeField] public UavEventChannelSO uavDestroyedEventChannel;
		[SerializeField] public UavEventChannelSO uavHiddenEventChannel;
		[SerializeField] public UavEventChannelSO uavVisibleEventChannel;
		[SerializeField] public UavEventChannelSO uavShotDownChannel;

		[SerializeField] public NavigationChannelsSO navigationChannels;
		[SerializeField] public FuelAndHealthChannelsSO fuelAndHealthChannels;
		[SerializeField] public UavPathEventChannelSO uavReroutedEventChannel;
		[SerializeField] public UavEventChannelSO reroutingOptionsRequestedChannel;

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavCreatedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavDestroyedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavHiddenEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavVisibleEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavShotDownChannel,this);
			AssertionHelper.AssertAssetReferenced(navigationChannels,this);
			AssertionHelper.AssertAssetReferenced(fuelAndHealthChannels,this);
			AssertionHelper.AssertAssetReferenced(uavReroutedEventChannel,this);
		}
	}
}