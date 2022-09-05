using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.EventChannels
{
	/// <summary>
	/// This class is used for Events that have one int argument.
	/// Example: An Achievement unlock event, where the int is the Achievement ID.
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
