using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	/// <summary>
	/// This class is used for Events that have one object argument.
	/// Example: An object (script) raises an event when it is created and notifies all subscribers about its creation
	/// </summary>
	[CreateAssetMenu(menuName = "Events/Object Event Channel")]
	public class ObjectEventChannelSO:ScriptableObject
	{
		private UnityAction<object> onEventRaised;
	
		public void RaiseEvent(object value)
		{
			if (onEventRaised != null)
				onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<object> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<object> action)
		{
			onEventRaised -= action;
		}
	}
}