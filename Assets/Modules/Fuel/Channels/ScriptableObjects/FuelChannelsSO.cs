using HelperScripts;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;

namespace Modules.FuelAndHealth.Channels.ScriptableObjects
{
	[CreateAssetMenu(fileName = "FuelChannels", menuName = "Channels/Fuel Channels")]

	public class FuelChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavFloatEventChannelSO uavFuelLevelChangedEventChannel;
		[SerializeField] public UavStringEventChannelSO uavHealthButtonClickedEventChannel;
		[SerializeField] public UavEventChannelSO uavFuelLeakFixedEventChannel;
		[SerializeField] public UavFuelConditionEventChannelSO uavFuelConditionChangedEventChannel;

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavFuelLevelChangedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavHealthButtonClickedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFuelLeakFixedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavFuelConditionChangedEventChannel,this);
		}
	}
}