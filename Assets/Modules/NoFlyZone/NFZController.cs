using System;
using System.Collections;
using DG.Tweening;
using HelperScripts;
using IOHandlers;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using TMPro;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;

namespace Modules.NoFlyZone
{
	public class NFZController:MonoBehaviour
	{
		public TextMeshPro textMeshPro;
		public GameObject cube;
		private NFZSettingsSO _settings;

		private Vector3 _startPosition;
		private Vector3 _endPosition;
		private Vector3 _center;
		private Vector3 _scale;
		private float _endTime;
		
		private float _nfzColorAlpha; //transparency level (1 - opaque, 0 - transparent)


		private UavEventChannelSO _uavLostEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		
		public void Initialize(NFZRecord nfzRecord)
		{

			_uavLostEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavLostEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
			_settings= GameManager.Instance.settingsDatabase.nfzSettings;
			
			_startPosition =  new Vector3(nfzRecord.StartCoordinates.X??0, nfzRecord.StartCoordinates.Y??0, nfzRecord.StartCoordinates.Z??0);
			_endPosition =  new Vector3(nfzRecord.EndCoordinates.X??0, nfzRecord.EndCoordinates.Y??0, nfzRecord.EndCoordinates.Z??0);
			
			//setting position
			_center = (_startPosition + _endPosition) / 2;
			cube.transform.position = _center;
			textMeshPro.transform.position = new Vector3(_center.x, _center.y*2, _center.z);

			//setting scale
			_scale = (_endPosition - _startPosition);
			textMeshPro.GetComponent<RectTransform>().sizeDelta = new Vector2(_scale.z, _scale.x); //flipped because of rotation
			if(_settings.animateNFZGrowOnStart)
			{
				cube.transform.DOScale(_scale, Math.Min(_settings.nfzGrowthAnimationDuration, nfzRecord.NFZCountdownTimer));
			}
			else
			{
				cube.transform.localScale = _scale;
			}
			
			//setting end time and making sure it gets destroyed when the end time is reached
			_endTime= nfzRecord.NFZEndTime;
			if (_endTime > 0)
				StartCoroutine(RemoveNFZ());
			
			
			if (nfzRecord.NFZCountdownTimer > 0) //checking if timer till start of NFZ is set
			{
				//set layer to default
				cube.layer = 0;
				cube.GetComponent<BoxCollider>().enabled = false; // disabling collider since the NFZ is not active yet
				StartCoroutine(EnableCollider(nfzRecord)); //enabling collider after the timer is over
			}
			else //we have no countdown
			{
				textMeshPro.text = nfzRecord.TextOnNFZAfterCountdown;
			}
			
			//blinker animation
			if(_settings.blinkNFZOnCountdownCounter && nfzRecord.NFZCountdownTimer > 0)
			{
				textMeshPro.text = _settings.nfzCountdownText;
				StartCoroutine(BlinkNFZ(nfzRecord));
			}
			
			if(_settings.nfzAddCountdownCounterToText&& nfzRecord.NFZCountdownTimer > 0)
			{
				StartCoroutine(AddCountdownToText(nfzRecord));
			}
		}

		private IEnumerator EnableCollider(NFZRecord nfzRecord) //enable collider after countdown and set text to what it should be after the countdown
		{
			yield return new WaitForSeconds(nfzRecord.NFZCountdownTimer);
			cube.GetComponent<BoxCollider>().enabled = true;
			cube.layer = LayerMask.NameToLayer("NFZ");
			textMeshPro.text = nfzRecord.TextOnNFZAfterCountdown;
		}

		private IEnumerator AddCountdownToText(NFZRecord nfzRecord)
		{
			while (Time.time < nfzRecord.NFZCountdownTimer+ nfzRecord.NFZStartTime+GameManager.Instance.simulationStartTime)
			{
				textMeshPro.text = _settings.nfzCountdownText + " "+ (nfzRecord.NFZCountdownTimer+ nfzRecord.NFZStartTime+GameManager.Instance.simulationStartTime - Time.time).ToString("F0");
				yield return new WaitForSeconds(1);
			}
		}

		private IEnumerator BlinkNFZ(NFZRecord nfzRecord)
		{
			_nfzColorAlpha= cube.GetComponent<Renderer>().material.color.a;// getting a reference to the initial transparency level so we can reset it after the animation
			var doTweenSequence = DOTween.Sequence();
			if (Time.time < nfzRecord.NFZCountdownTimer + nfzRecord.NFZStartTime+GameManager.Instance.simulationStartTime)
			{
				doTweenSequence.Append(cube.GetComponent<Renderer>().material.DOFade(0, _settings.nfzBlinkInterval));
				doTweenSequence.SetLoops(-1, LoopType.Yoyo);
				yield return new WaitForSeconds(nfzRecord.NFZCountdownTimer);
			}
			
			doTweenSequence.Kill();
			cube.GetComponent<Renderer>().material.DOFade(_nfzColorAlpha, 0.1f); //resetting to default (i.e.,visible)
			
		}

		public IEnumerator RemoveNFZ()
		{
			if (Time.time < _endTime+GameManager.Instance.simulationStartTime)
			{
				yield return new WaitForSeconds(_endTime+GameManager.Instance.simulationStartTime - Time.time);
			}
			Destroy(gameObject);
			
		}

		private void OnTriggerEnter(Collider collision)
		{
			if (!collision.gameObject.CompareTag("UAV")) return;
			
			var uav = collision.gameObject.GetComponentInParent<Uav>();
			if (uav == null) return; //sanity check, since we already got that it has a tag of UAV, but we are double checking here that it has a Uav component
				
			if ( _uavLostEventChannel!= null)
				_uavLostEventChannel.RaiseEvent(uav);
				
			if (_uavConditionChangedEventChannel!= null)
				_uavConditionChangedEventChannel.RaiseEvent(uav, Enums.UavCondition.Lost);
		}
	}
}