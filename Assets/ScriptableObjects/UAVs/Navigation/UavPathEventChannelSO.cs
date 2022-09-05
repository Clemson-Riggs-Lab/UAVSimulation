using UAVs;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.UAVs.Navigation
{

	[CreateAssetMenu(menuName = "Events/Uav Sub-Modules/Uav Path Event Channel")]
	public class UavPathEventChannelSO : ScriptableObject
	{
		
		private UnityAction<Uav,Path> onEventRaised;
	
		public void RaiseEvent(Uav uav,Path path)
		{
			onEventRaised?.Invoke(uav, path);
		}

		public void Subscribe(UnityAction<Uav,Path> action)
		{
			onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,Path> action)
		{
			onEventRaised -= action;
		}
	}
}
