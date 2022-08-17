using System;
using UnityEngine.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Events.ScriptableObjects
{
	/// <summary>
	/// This class is used for Events that have one object argument.
	/// Example: An object (script) raises an event when it is created and notifies all subscribers about its creation
	/// </summary>
	[CreateAssetMenu(menuName = "Events/Object Event Channel")]
	public class ObjectEventChannelSO:ScriptableObject
	{
		public UnityAction<object> OnEventRaised;
	
		public void RaiseEvent(object value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<object> action)
		{
			OnEventRaised += action;
		}

		public void Unsubscribe(UnityAction<object> action)
		{
			OnEventRaised -= action;
		}
	}
}