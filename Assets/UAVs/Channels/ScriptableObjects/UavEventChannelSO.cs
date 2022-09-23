using UnityEngine;
using UnityEngine.Events;

namespace UAVs.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Uav Event Channel")]
	public class UavEventChannelSO:ScriptableObject
	{
		
		private UnityAction<Uav> _onEventRaised;

		public void RaiseEvent(Uav value)
		{
			_onEventRaised?.Invoke(value);
		}
		public void Subscribe(UnityAction<Uav> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav> action)
		{
			_onEventRaised -= action;
		}
	}
}
	
