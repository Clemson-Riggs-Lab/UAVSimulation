using System;
using System.Collections;
using Databases.ScriptableObjects;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Console
{ 
	public class SimulationControllerManager : MonoBehaviour
	{
		[SerializeField] Button _pauseBtn;
		[SerializeField] TextMeshProUGUI _pauseBtnTxt;

		private void OnEnable()
		{
			_pauseBtn.onClick.AddListener(OnClickPause);
		}

		private void OnDisable()
		{
            _pauseBtn.onClick.RemoveListener(OnClickPause);
        }

		private void Start()
		{
			if (GameManager.Instance.PauseStatus)
				_pauseBtnTxt.text = "Resume";
			else
				_pauseBtnTxt.text = "Pause";

			GameplayNetworkCallsHandler.Instance.PauseBehaviour_NetworkEventHandler += OnPauseBehaviourNetworkEventHandler;
		}

		private void OnDestroy()
		{
            GameplayNetworkCallsHandler.Instance.PauseBehaviour_NetworkEventHandler -= OnPauseBehaviourNetworkEventHandler;
        }

		private void OnClickPause()
		{
			GameplayNetworkCallsHandler.Instance.PauseBehaviourServerRpc(GameManager.Instance.PauseStatus ? false : true);
        }

        private void OnPauseBehaviourNetworkEventHandler(object sender, bool pauseStatus)
        {
            GameManager.Instance.ChangePauseStatus(pauseStatus);

            _pauseBtnTxt.text = GameManager.Instance.PauseStatus ? _pauseBtnTxt.text = "Resume" : _pauseBtnTxt.text = "Pause";
        }
    }
}