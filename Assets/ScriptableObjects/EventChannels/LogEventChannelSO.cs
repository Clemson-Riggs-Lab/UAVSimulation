using Logging;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	[CreateAssetMenu(menuName = "Events/Log Event Channel")]
	public class LogEventChannelSO:ScriptableObject
	{
		private UnityAction<Log> onEventRaised;
	
		public void RaiseEvent(Log value)
		{
			if (onEventRaised != null)
				onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<Log> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Log> action)
		{
			onEventRaised -= action;
		}
	}
}