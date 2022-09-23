using UnityEngine;
using UnityEngine.Events;
using static HelperScripts.Enums;

namespace UAVs.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/UavCondition Event Channel")]
	public class UavConditionEventChannelSO: ScriptableObject
	{
		private UnityAction<Uav,UavCondition> _onEventRaised;
	
		public void RaiseEvent(Uav uav,UavCondition condition)
		{
			_onEventRaised?.Invoke(uav, condition);
		}

		public void Subscribe(UnityAction<Uav,UavCondition> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,UavCondition> action)
		{
			_onEventRaised -= action;
		}
	}
}