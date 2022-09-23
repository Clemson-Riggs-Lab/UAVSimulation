using UnityEngine;
using UnityEngine.Events;

namespace UAVs.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/UavString Event Channel")]
	public class UavStringEventChannelSO: ScriptableObject
	{
		private UnityAction<Uav,string> _onEventRaised;
	
		public void RaiseEvent(Uav uav,string str)
		{
			_onEventRaised?.Invoke(uav, str);
		}

		public void Subscribe(UnityAction<Uav,string> str)
		{
			_onEventRaised += str;
		}

		public void Unsubscribe(UnityAction<Uav,string> str)
		{
			_onEventRaised -= str;
		}
	}
}