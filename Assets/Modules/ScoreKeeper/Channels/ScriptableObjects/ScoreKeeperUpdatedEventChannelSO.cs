using UnityEngine;
using UnityEngine.Events;
using Score = Modules.ScoreKeeper.Score;

namespace Modules.ScoreKeeper.Channels.ScriptableObjects
{

	[CreateAssetMenu(menuName = "Events/Score Keeper Updated Event Channel")]
	public class ScoreKeeperUpdatedEventChannelSO : ScriptableObject
	{
		
		private UnityAction<Score> _onEventRaised;
	
		public void RaiseEvent(Score score)
		{
			_onEventRaised?.Invoke(score);
		}

		public void Subscribe(UnityAction<Score> action)
		{
			_onEventRaised += action;
		}

		public void Unsubscribe(UnityAction<Score> action)
		{
			_onEventRaised -= action;
		}
	}
}
