using UnityEngine;
using UnityEngine.Events;

namespace Modules.Prompts.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Prompts/ Response Option Event Channel")]
	public class ResponseOptionEventChannelSO:ScriptableObject
	{
		private UnityAction<ResponseOption> _onEventRaised;
	
		public void RaiseEvent(ResponseOption value)
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<ResponseOption> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<ResponseOption> action)
		{
			_onEventRaised -= action;
		}
	}

}