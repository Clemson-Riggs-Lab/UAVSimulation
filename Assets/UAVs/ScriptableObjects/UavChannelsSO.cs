using Events.ScriptableObjects;
using HelperScripts;
using UAVs.Sub_Modules.Navigation.Channels;
using UnityEngine;

namespace Chat.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UavChannelsSO", menuName = "Channels/Uav Channels")]

	public class UavChannelsSO:ScriptableObject
	{
		//all fields below should be  set in inspector
		[Space(20)]
		[SerializeField] public ObjectEventChannelSO uavCreatedEventChannel;
		[SerializeField] public ObjectEventChannelSO uavDestroyedEventChannel;
		[SerializeField] public ObjectEventChannelSO uavHiddenEventChannel;
		[SerializeField] public ObjectEventChannelSO uavVisibleEventChannel;
		[SerializeField] public ObjectEventChannelSO uavShotDownChannel;

		[SerializeField] public NavigationChannelsSO navigationChannels;
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
		}
	}
}