using Logging;
using UAVs;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	[CreateAssetMenu(menuName = "Events/Uav Event Channel")]
	public class UavEventChannelSO:ScriptableObject
	{
		
		private UnityAction<Uav> onEventRaised;

		public void RaiseEvent(Uav value)
		{
			onEventRaised?.Invoke(value);
		}
		public void Subscribe(UnityAction<Uav> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav> action)
		{
			onEventRaised -= action;
		}
	}
}
	
