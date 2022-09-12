using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	/// <summary>
	/// This class is used for Events that have one int argument.
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Int Event Channel")]
	public class IntEventChannelSO : ScriptableObject
	{
		private UnityAction<int> onEventRaised;
	
		public void RaiseEvent(int value)
		{
			if (onEventRaised != null)
				onEventRaised.Invoke(value);
		}
		
		
	}
}
