using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
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
		}

		public void LoadNFZs()
		{
			switch (nfzSettings.nfzRecordsSource)
			{
				case Enums.InputRecordsSource.FromInputFile:
					nfzRecords = GameManager.Instance.inputRecordsDatabase.NFZRecords;
					break;
				case Enums.InputRecordsSource.FromDefaultRecords:
					nfzRecords = DefaultRecordsCreator.GetDefaultNFZRecords();
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
			}
		}

		public IEnumerator StartNFZsTimerCoroutine(float simulationStartTime)
		{
			if (nfzRecords == null)
			{
				Debug.LogError("No Fly Zone records");
				yield break;
			}
			
			yield return new WaitForSeconds(simulationStartTime- Time.time);
			foreach (var nfzRecord in nfzRecords)
			{
				var deltaTime = nfzRecord.NFZStartTime+simulationStartTime - Time.time;
				if(deltaTime>0)
					yield return new WaitForSeconds(deltaTime);
				GenerateNFZ(nfzRecord);
			}
		}

		private void GenerateNFZ(NFZRecord nfzRecord)
		{
			var nfzController = Instantiate(nfzPrefab, nfzsContainer.transform).GetComponent<NFZController>();
				nfzController.Initialize(nfzRecord);
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