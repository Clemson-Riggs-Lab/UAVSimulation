using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	[CreateAssetMenu(menuName = "Events/String Event Channel")]
	public class StringEventChannelSO:ScriptableObject
	{
		private UnityAction<string> onEventRaised;
	
		public void RaiseEvent(string value)
		{
			if (onEventRaised != null)
				onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<string> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<string> action)
		{
			onEventRaised -= action;
		}
	}
}