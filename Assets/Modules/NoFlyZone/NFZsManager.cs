using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using Modules.NoFlyZone.Settings;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using UnityEngine;

namespace Modules.NoFlyZone
{
	public class NFZsManager:MonoBehaviour
	{
		private GameObject _nfzsContainer;
		private NFZSettingsSO _nfzSettings;
		private GameObject _nfzPrefab;
		
		private List<NFZRecord> _nfzRecords;

		public void Initialize()
		{
			GetReferencesFromGameManager();
			ClearNFZs();
			LoadNFZs();
			
			var nfzLogHandler = gameObject.GetOrAddComponent<NFZLogHandler>();
			nfzLogHandler.Initialize();
			
			
		}

		private void GetReferencesFromGameManager()
		{
			_nfzsContainer= GameManager.Instance.nfzsContainer;
			_nfzSettings = GameManager.Instance.settingsDatabase.nfzSettings;
			_nfzPrefab = GameManager.Instance.prefabsDatabase.nfzPrefab;
		}

		public void LoadNFZs()
		{
			switch (_nfzSettings.nfzRecordsSource)
			{
				case Enums.InputRecordsSource.FromInputFile:
					_nfzRecords = GameManager.Instance.inputRecordsDatabase.NFZRecords;
					break;
				case Enums.InputRecordsSource.FromDefaultRecords:
					_nfzRecords = DefaultRecordsCreator.GetDefaultNFZRecords();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (_nfzRecords == null)
			{
				Debug.LogError("No Fly Zone records are null");
				return;
			}
			else
			{
				//sort nfz records by  NFZStartTime 
				_nfzRecords = _nfzRecords.OrderBy(nfzRecord => nfzRecord.NFZStartTime).ToList();
			}
		}

		public IEnumerator StartNFZsTimerCoroutine(float simulationStartTime)
		{
			if (_nfzRecords == null)
			{
				Debug.LogError("No Fly Zone records");
				yield break;
			}
			
			yield return new WaitForSeconds(simulationStartTime- Time.time);
			foreach (var nfzRecord in _nfzRecords)
			{
				var deltaTime = nfzRecord.NFZStartTime+simulationStartTime - Time.time;
				if(deltaTime>0)
					yield return new WaitForSeconds(deltaTime);
				GenerateNFZ(nfzRecord);
			}
		}

		private void GenerateNFZ(NFZRecord nfzRecord)
		{
			var nfzController = Instantiate(_nfzPrefab, _nfzsContainer.transform).GetComponent<NFZController>();
				nfzController.Initialize(nfzRecord);
		}

		private void ClearNFZs()
		{
			foreach (Transform child in _nfzsContainer.transform)
			{
				Destroy(child.gameObject);
			}
		}
		
	}
}