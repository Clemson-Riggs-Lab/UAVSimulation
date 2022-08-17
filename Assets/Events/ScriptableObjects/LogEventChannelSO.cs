using System;
using Logging;
using UnityEngine.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Events.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Log Event Channel")]
	public class LogEventChannelSO:ScriptableObject
	{
		public UnityAction<Log> OnEventRaised;
	
		public void RaiseEvent(Log value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<Log> action)
		{
			OnEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Log> action)
		{
			OnEventRaised -= action;
		}
	}
}