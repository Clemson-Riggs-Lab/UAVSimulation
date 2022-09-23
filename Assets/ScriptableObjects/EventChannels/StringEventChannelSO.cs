using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	[CreateAssetMenu(menuName = "Events/String Event Channel")]
	public class StringEventChannelSO:ScriptableObject
	{
		private UnityAction<string> _onEventRaised;
	
		public void RaiseEvent(string value)
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<string> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<string> action)
		{
			_onEventRaised -= action;
		}
	}
}