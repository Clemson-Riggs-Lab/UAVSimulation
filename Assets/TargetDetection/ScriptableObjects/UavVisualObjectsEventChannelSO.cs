using System.Collections.Generic;
using UAVs;
using UnityEngine;
using UnityEngine.Events;

namespace TargetDetection.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Uav-VisualObjects Event Channel")]
	public class UavVisualObjectsEventChannelSO: ScriptableObject
	{
		public UnityAction<Uav,List<VisualObject>> OnEventRaised;
	
		public void RaiseEvent(Uav uav,List<VisualObject> visualObjects)
		{
			OnEventRaised?.Invoke(uav, visualObjects);
		}

		public void Subscribe(UnityAction<Uav,List<VisualObject>> action)
		{
			OnEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,List<VisualObject>> action)
		{
			OnEventRaised -= action;
		}
	}
}