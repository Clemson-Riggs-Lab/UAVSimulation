using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IOHandlers.Records;
using ScriptableObjects.EventChannels;
using ScriptableObjects.NoFlyZone;
using TMPro;
using UAVs;
using UnityEngine;

namespace NoFlyZone
{
	public class NFZController:MonoBehaviour
	{
		public TextMeshPro textMeshPro;
		public GameObject cube;
		private NFZSettingsSO settings;

		private Vector3 startPosition;
		private Vector3 endPosition;
		private Vector3 center;
		private Vector3 scale;
		private float endTime;
		
		private float nfzColorAlpha; //transparency level (1 - opaque, 0 - transparent)


		private UavEventChannelSO uavShotDownChannel;
		
		public void Initialize(NFZRecord nfzRecord)
		{

			uavShotDownChannel = GameManager.Instance.channelsDatabase.uavChannels.uavShotDownChannel;
			settings= GameManager.Instance.settingsDatabase.nfzSettings;
			
			startPosition =  new Vector3(nfzRecord.StartCoordinates.X??0, nfzRecord.StartCoordinates.Y??0, nfzRecord.StartCoordinates.Z??0);
			endPosition =  new Vector3(nfzRecord.EndCoordinates.X??0, nfzRecord.EndCoordinates.Y??0, nfzRecord.EndCoordinates.Z??0);
			
			//setting position
			center = (startPosition + endPosition) / 2;
			cube.transform.position = center;
			textMeshPro.transform.position = new Vector3(center.x, center.y*2, center.z);

			//setting scale
			scale = (endPosition - startPosition);
			textMeshPro.GetComponent<RectTransform>().sizeDelta = new Vector2(scale.z, scale.x); //flipped because of rotation
			if(settings.animateNFZGrowOnStart)
			{
				cube.transform.DOScale(scale, Math.Min(settings.nfzGrowthAnimationDuration, nfzRecord.NFZCountdownTimer));
			}
			else
			{
				cube.transform.localScale = scale;
			}
			
			//setting end time and making sure it gets destroyed when the end time is reached
			endTime= nfzRecord.NFZEndTime;
			if (endTime > 0)
				StartCoroutine(RemoveNFZ());
			
			
			if (nfzRecord.NFZCountdownTimer > 0) //checking if timer till start of NFZ is set
			{
				cube.GetComponent<BoxCollider>().enabled = false; // disabling collider since the NFZ is not active yet
				StartCoroutine(EnableCollider(nfzRecord)); //enabling collider after the timer is over
			}
			else //we have no countdown
			{
				textMeshPro.text = nfzRecord.TextOnNFZAfterCountdown;
			}
			
			//blinker animation
			if(settings.blinkNFZOnCountdownCounter && nfzRecord.NFZCountdownTimer > 0)
			{
				textMeshPro.text = settings.nfzCountdownText;
				StartCoroutine(BlinkNFZ(nfzRecord));
			}
			
			if(settings.nfzAddCountdownCounterToText&& nfzRecord.NFZCountdownTimer > 0)
			{
				StartCoroutine(AddCountdownToText(nfzRecord));
			}
		}

		private IEnumerator EnableCollider(NFZRecord nfzRecord) //enable collider after countdown and set text to what it should be after the countdown
		{
			yield return new WaitForSeconds(nfzRecord.NFZCountdownTimer);
			cube.GetComponent<BoxCollider>().enabled = true;
			textMeshPro.text = nfzRecord.TextOnNFZAfterCountdown;
		}

		private IEnumerator AddCountdownToText(NFZRecord nfzRecord)
		{
			while (Time.time < nfzRecord.NFZCountdownTimer+ nfzRecord.NFZStartTime+GameManager.Instance.simulationStartTime)
			{
				textMeshPro.text = settings.nfzCountdownText + " "+ (nfzRecord.NFZCountdownTimer+ nfzRecord.NFZStartTime+GameManager.Instance.simulationStartTime - Time.time).ToString("F0");
				yield return new WaitForSeconds(1);
			}
		}

		private IEnumerator BlinkNFZ(NFZRecord nfzRecord)
		{
			nfzColorAlpha= cube.GetComponent<Renderer>().material.color.a;// getting a reference to the initial transparency level so we can reset it after the animation
			var doTweenSequence = DOTween.Sequence();
			if (Time.time < nfzRecord.NFZCountdownTimer + nfzRecord.NFZStartTime+GameManager.Instance.simulationStartTime)
			{
				doTweenSequence.Append(cube.GetComponent<Renderer>().material.DOFade(0, settings.nfzBlinkInterval));
				doTweenSequence.SetLoops(-1, LoopType.Yoyo);
				yield return new WaitForSeconds(nfzRecord.NFZCountdownTimer);
			}
			
			doTweenSequence.Kill();
			cube.GetComponent<Renderer>().material.DOFade(nfzColorAlpha, 0.1f); //resetting to default (i.e.,visible)
			
		}

		public IEnumerator RemoveNFZ()
		{
			if (Time.time < endTime+GameManager.Instance.simulationStartTime)
			{
				yield return new WaitForSeconds(endTime+GameManager.Instance.simulationStartTime - Time.time);
			}
			Destroy(gameObject);
			
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("UAV"))
			{
				var uav = collision.gameObject.GetComponentInParent<Uav>();
				if (uav != null && uavShotDownChannel!= null)
				{
					uavShotDownChannel.RaiseEvent(uav);
				}
			}
		}

		private void OnTriggerEnter(Collider collision)
		{
			if (collision.gameObject.CompareTag("UAV"))
			{
				var uav = collision.gameObject.GetComponent<Uav>();
				if (uav != null && uavShotDownChannel!= null)
				{
					uavShotDownChannel.RaiseEvent(uav);
				}
			}
		}
	}
}