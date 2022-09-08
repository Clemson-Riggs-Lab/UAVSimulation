using System.Collections.Generic;
using TargetDetection;
using UAVs;
using UnityEngine;
using UnityEngine.Events;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO;

namespace ScriptableObjects.EventChannels
{
	[CreateAssetMenu(menuName = "Events/Uav-UavHealthCondition Event Channel")]
	public class Uav_UavHealthConditionEventChannelSO: ScriptableObject
	{
		private UnityAction<Uav,UavHealthConditions> onEventRaised;
	
		public void RaiseEvent(Uav uav,UavHealthConditions condition)
		{
			onEventRaised?.Invoke(uav, condition);
		}

		public void Subscribe(UnityAction<Uav,UavHealthConditions> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,UavHealthConditions> action)
		{
			onEventRaised -= action;
		}
	}
}