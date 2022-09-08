using HelperScripts;
using ScriptableObjects.EventChannels;
using UnityEngine;

namespace ScriptableObjects.UAVs.FuelAndHealth
{
	[CreateAssetMenu(fileName = "FuelAndHealthChannelsSO", menuName = "Channels/Uav Sub-Modules/FuelAndHealth Channels")]

	public class FuelAndHealthChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]

		[SerializeField] public UavEventChannelSO uavLostEventChannel;
		[SerializeField] public UavEventChannelSO uavHealthyEventChannel;
         
		[SerializeField] public UavEventChannelSO uavFuelEmptyEventChannel;
		[SerializeField] public UavEventChannelSO uavFuelLeakingEventChannel;
		[SerializeField] public UavEventChannelSO uavFuelLeakFixedEventChannel;
		[SerializeField] public UavEventChannelSO uavFatalFuelLeakEventChannel;
		
		[SerializeField] public Uav_FuelConditionEventChannelSO uavFuelConditionChangedEventChannel;
		[SerializeField] public Uav_UavHealthConditionEventChannelSO uavHealthConditionChangedEventChannel;

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavLostEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavHealthyEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFuelEmptyEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFuelLeakingEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFuelLeakFixedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFatalFuelLeakEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFuelConditionChangedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavHealthConditionChangedEventChannel,this);
		}
	}
}