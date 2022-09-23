using Modules.Prompts;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Console.Channels.ScriptableObjects
{

	[CreateAssetMenu(menuName = "Events/Console Text Event Channel")]
	public class ConsoleMessageEventChannelSO : ScriptableObject
	{
		
		private UnityAction<string,ConsoleMessage> _onEventRaised;
	
		public void RaiseEvent(string prefix,ConsoleMessage message)
		{
			_onEventRaised?.Invoke(prefix, message);
		}

		public void Subscribe(UnityAction<string,ConsoleMessage> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<string,ConsoleMessage> action)
		{
			_onEventRaised -= action;
		}
	}
}
