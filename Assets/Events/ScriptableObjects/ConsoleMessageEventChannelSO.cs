using Prompts;
using UnityEngine;
using UnityEngine.Events;

namespace Events.ScriptableObjects
{

	[CreateAssetMenu(menuName = "Events/Console Text Event Channel")]
	public class ConsoleMessageEventChannelSO : ScriptableObject
	{
		
		public UnityAction<string,ConsoleMessage> OnEventRaised;
	
		public void RaiseEvent(string prefix,ConsoleMessage message)
		{
			OnEventRaised?.Invoke(prefix, message);
		}

		public void Subscribe(UnityAction<string,ConsoleMessage> action)
		{
			OnEventRaised += action;
		}

		public void Unsubscribe(UnityAction<string,ConsoleMessage> action)
		{
			OnEventRaised -= action;
		}
	}
}
