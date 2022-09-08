using System.Collections.Generic;
using TargetDetection;
using UAVs;
using UnityEngine;
using UnityEngine.Events;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO;

namespace ScriptableObjects.EventChannels
{
	[CreateAssetMenu(menuName = "Events/Uav-FuelCondition Event Channel")]
	public class Uav_FuelConditionEventChannelSO: ScriptableObject
	{
		private UnityAction<Uav,FuelConditions> onEventRaised;
	
		public void RaiseEvent(Uav uav,FuelConditions condition)
		{
			onEventRaised?.Invoke(uav, condition);
		}

		public void Subscribe(UnityAction<Uav,FuelConditions> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,FuelConditions> action)
		{
			onEventRaised -= action;
		}
	}
}