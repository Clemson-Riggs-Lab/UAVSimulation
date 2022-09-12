using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IOHandlers.Records;
using ScriptableObjects.NoFlyZone;
using UnityEngine;

namespace NoFlyZone
{
	public class NFZsManager:MonoBehaviour
	{
		private GameObject nfzsContainer;
		private NFZSettingsSO nfzSettings;
		private GameObject nfzPrefab;
		
		private List<NFZRecord> nfzRecords;

		public void Initialize()
		{
			nfzsContainer= GameManager.Instance.nfzsContainer;
			nfzSettings = GameManager.Instance.settingsDatabase.nfzSettings;
			nfzPrefab = GameManager.Instance.prefabsDatabase.nfzPrefab;
			ClearNFZs();
			LoadNFZs();
		}

		private void LoadNFZs()
		{
			switch(nfzSettings.nfzRecordsSource)
			{
				case NFZSettingsSO.NFZRecordsSource.Disabled:
					Destroy(nfzsContainer.gameObject);
					Destroy(this);
					return;
				case NFZSettingsSO.NFZRecordsSource.FromFile:
					//TODO: Load from file
					break;
				case NFZSettingsSO.NFZRecordsSource.FromDefaultRecords:
					nfzRecords = DefaultRecordsCreator.AddDefaultNFZRecords();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			if (nfzRecords == null)
			{
				Debug.LogError("No Fly Zone records are null");
				return;
			}
			else
			{
				//sort nfz records by  NFZStartTime 
				nfzRecords = nfzRecords.OrderBy(nfzRecord => nfzRecord.NFZStartTime).ToList();
				StartCoroutine(StartNFZTimerCoroutine()); 
			}
		}

		private IEnumerator StartNFZTimerCoroutine()
		{
			foreach (var nfzRecord in nfzRecords)
			{
				var deltaTime = nfzRecord.NFZStartTime - Time.time;
				if(deltaTime>0)
					yield return new WaitForSeconds(deltaTime);
				GenerateNFZ(nfzRecord);
			}
		}

		private void GenerateNFZ(NFZRecord nfzRecord)
		{
			if (nfzRecord != null)
			{
				var nfzController = Instantiate(nfzPrefab, nfzsContainer.transform).GetComponent<NFZController>();
				nfzController.Initialize(nfzRecord);
			}
			else
			{
				Debug.LogError("NFZ record is null");
			}
		}

		private void ClearNFZs()
		{
			foreach (Transform child in nfzsContainer.transform)
			{
				Destroy(child.gameObject);
			}
		}
	}
}