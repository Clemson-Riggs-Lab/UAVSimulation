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
		private UnityAction<int> _onEventRaised;
	
		public void RaiseEvent(int value)
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke(value);
		}
		
		
	}
}
