using UnityEngine;
using UnityEngine.Events;

namespace Modules.Prompts.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Prompts/Prompt Event Channel")]
	public class PromptEventChannelSO:ScriptableObject
	{
		private UnityAction<Prompt> _onEventRaised;
	
		public void RaiseEvent(Prompt value)
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<Prompt> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Prompt> action)
		{
			_onEventRaised -= action;
		}
	}
}