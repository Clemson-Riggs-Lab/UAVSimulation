using System;
using UnityEngine.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Events.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/String Event Channel")]
	public class StringEventChannelSo:ScriptableObject
	{
		public UnityAction<string> OnEventRaised;
	
		public void RaiseEvent(string value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<string> action)
		{
			OnEventRaised += action;
		}

		public void Unsubscribe(UnityAction<string> action)
		{
			OnEventRaised -= action;
		}
	}
}