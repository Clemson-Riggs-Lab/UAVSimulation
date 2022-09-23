using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	/// <summary>
	/// This class is used for Events that have no arguments (Example: Exit game event)
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Void Event Channel")]
	public class VoidEventChannelSO : ScriptableObject
	{
		private UnityAction _onEventRaised;

		public void RaiseEvent()
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke();
		}
		
		public void Subscribe(UnityAction action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction action)
		{
			_onEventRaised -= action;
		}
	}
}


