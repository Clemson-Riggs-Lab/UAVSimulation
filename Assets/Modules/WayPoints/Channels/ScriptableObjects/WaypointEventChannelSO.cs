using UnityEngine;
using UnityEngine.Events;

namespace WayPoints.Channels.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Events/Waypoint Event Channel")]
	public class WaypointEventChannelSO:ScriptableObject
	{
		private UnityAction<WayPoint> _onEventRaised;
	
		public void RaiseEvent(WayPoint value)
		{
			if (_onEventRaised != null)
				_onEventRaised.Invoke(value);
		}
		public void Subscribe(UnityAction<WayPoint> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<WayPoint> action)
		{
			_onEventRaised -= action;
		}
	}
	
}