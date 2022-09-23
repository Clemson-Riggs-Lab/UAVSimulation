using HelperScripts;
using Modules.FuelAndHealth.Channels.ScriptableObjects;
using Modules.Navigation.Channels.ScriptableObjects;
using UnityEngine;

namespace UAVs.Channels.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UavChannelsSO", menuName = "Channels/Uav Channels")]

	public class UavChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public UavEventChannelSO uavCreatedEventChannel;
		[SerializeField] public UavEventChannelSO uavDestroyedEventChannel;
		[SerializeField] public UavEventChannelSO uavLostEventChannel;
		[SerializeField] public UavEventChannelSO uavShotDownChannel;
		[SerializeField] public UavConditionEventChannelSO uavConditionChangedEventChannel;


		

		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavCreatedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavDestroyedEventChannel,this);
			AssertionHelper.AssertAssetReferenced(uavShotDownChannel,this);
			AssertionHelper.AssertAssetReferenced(uavConditionChangedEventChannel,this);
		}
	}
}