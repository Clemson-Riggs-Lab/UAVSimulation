using UnityEngine;
using UnityEngine.Events;

namespace Modules.Logging.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Log Event Channel")]
	public class LogEventChannelSO:ScriptableObject
	{
		private UnityAction<Log> _onEventRaised;
	
		public void RaiseEvent(Log value)
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<Log> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Log> action)
		{
			_onEventRaised -= action;
		}
	}
}