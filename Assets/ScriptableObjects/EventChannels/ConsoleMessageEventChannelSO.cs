using Prompts;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{

	[CreateAssetMenu(menuName = "Events/Console Text Event Channel")]
	public class ConsoleMessageEventChannelSO : ScriptableObject
	{
		
		private UnityAction<string,ConsoleMessage> onEventRaised;
	
		public void RaiseEvent(string prefix,ConsoleMessage message)
		{
			onEventRaised?.Invoke(prefix, message);
		}

		public void Subscribe(UnityAction<string,ConsoleMessage> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<string,ConsoleMessage> action)
		{
			onEventRaised -= action;
		}
	}
}
