using UAVs;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.Navigation.Channels.ScriptableObjects
{

	[CreateAssetMenu(menuName = "Events/Uav Path Event Channel")]
	public class UavPathEventChannelSO : ScriptableObject
	{
		
		private UnityAction<Uav,Path> _onEventRaised;
	
		public void RaiseEvent(Uav uav,Path path)
		{
			_onEventRaised?.Invoke(uav, path);
		}

		public void Subscribe(UnityAction<Uav,Path> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Uav,Path> action)
		{
			_onEventRaised -= action;
		}
	}
}
