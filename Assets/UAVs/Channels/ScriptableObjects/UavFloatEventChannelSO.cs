using UnityEngine;
using UnityEngine.Events;

namespace UAVs.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/UavFloat Event Channel")]
	public class UavFloatEventChannelSO: ScriptableObject
	{
		private UnityAction<Uav,float> _onEventRaised;
	
		public void RaiseEvent(Uav uav,float arg0)
		{
			_onEventRaised?.Invoke(uav, arg0);
		}

		public void Subscribe(UnityAction<Uav,float> arg0)
		{
			_onEventRaised += arg0;
		}

		public void Unsubscribe(UnityAction<Uav,float> arg0)
		{
			_onEventRaised -= arg0;
		}
	}
}