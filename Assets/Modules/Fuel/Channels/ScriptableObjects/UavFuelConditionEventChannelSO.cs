using UAVs;
using UnityEngine;
using UnityEngine.Events;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;

namespace Modules.FuelAndHealth.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/UavFuelCondition Event Channel")]
	public class UavFuelConditionEventChannelSO: ScriptableObject
	{
		private UnityAction<Uav,FuelCondition> _onEventRaised;
	
		public void RaiseEvent(Uav uav,FuelCondition condition)
		{
			_onEventRaised?.Invoke(uav, condition);
		}

		public void Subscribe(UnityAction<Uav,FuelCondition> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,FuelCondition> action)
		{
			_onEventRaised -= action;
		}
	}
}