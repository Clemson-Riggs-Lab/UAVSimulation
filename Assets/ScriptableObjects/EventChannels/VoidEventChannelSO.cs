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
		private UnityAction onEventRaised;

		public void RaiseEvent()
		{
			if (onEventRaised != null)
				onEventRaised.Invoke();
		}
	}
}


