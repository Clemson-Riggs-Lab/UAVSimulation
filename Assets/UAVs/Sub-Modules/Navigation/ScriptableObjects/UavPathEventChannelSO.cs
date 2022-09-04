using UnityEngine;
using UnityEngine.Events;

namespace UAVs.Sub_Modules.Navigation.ScriptableObjects
{

	[CreateAssetMenu(menuName = "Events/Uav Sub-Modules/Uav Path Event Channel")]
	public class UavPathEventChannelSO : ScriptableObject
	{
		
		public UnityAction<Uav,Path> OnEventRaised;
	
		public void RaiseEvent(Uav uav,Path path)
		{
			OnEventRaised?.Invoke(uav, path);
		}

		public void Subscribe(UnityAction<Uav,Path> action)
		{
			OnEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,Path> action)
		{
			OnEventRaised -= action;
		}
	}
}
