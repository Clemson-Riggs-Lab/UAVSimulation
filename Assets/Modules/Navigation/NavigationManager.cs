using System;
using System.Collections;
using System.Collections.Generic;
using IOHandlers;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.InputRecordsSource;
using static HelperScripts.Enums.UavCondition;

namespace Modules.Navigation
{
	public class NavigationManager: MonoBehaviour
	{
		private PathsGenerator _pathsGenerator ;
		private NavigationSettingsSO _navigationSettings;
		 private List<UavPathsRecord>  _uavPathsRecords;
		 
		 private UavPathEventChannelSO _uavReroutedEventChannel;
		 private UavEventChannelSO _uavDestroyedEventChannel;
		 private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		 
		 [NonSerialized] public Dictionary<Uav,Navigator> uavsToNavigatorsDictionary = new ();
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			
			// paths generator
			_pathsGenerator = gameObject.AddComponent<PathsGenerator>();
			_pathsGenerator.Initialize();
			GeneratePaths();
		}
		

		private void OnUavDestroyed(Uav uav)
		{
			//destroy navigator and remove it from the dictionary
			if (uavsToNavigatorsDictionary.ContainsKey(uav))
			{
				Destroy(uavsToNavigatorsDictionary[uav]);
				uavsToNavigatorsDictionary.Remove(uav);
			}
		}

		private void OnUavConditionChanged(Uav uav, UavCondition uavCondition)
		{
			//destroy navigator and remove it from the dictionary if the uav is lost or finished its navigation
			if (uavCondition is not (Lost or Finished)) return;
			if (uavsToNavigatorsDictionary.ContainsKey(uav))
			{
				Destroy(uavsToNavigatorsDictionary[uav]);
				uavsToNavigatorsDictionary.Remove(uav);
			}
		}

		private void RerouteUav(Uav uav, Path path)
		{
			//check if uavsToNavigatorsDictionary dictionary contains the uav
			if (uavsToNavigatorsDictionary.ContainsKey(uav)) 
				uavsToNavigatorsDictionary[uav].Reroute(path); //if it does, reroute the uav
		}

		

		public void GeneratePaths()
		{
			switch(_navigationSettings.navigationRecordsSource)
			{
				case FromDefaultRecords:
					_uavPathsRecords = DefaultRecordsCreator.GetDefaultUavPathsRecords();
					uavsToNavigatorsDictionary = _pathsGenerator.GeneratePaths(_uavPathsRecords);
					break;
				case FromInputFile:
					_uavPathsRecords = GameManager.Instance.inputRecordsDatabase.UavPathsRecords;
					uavsToNavigatorsDictionary = _pathsGenerator.GeneratePaths(_uavPathsRecords);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		
		}

		public IEnumerator NavigateAll(float simulationStartTime)
		{
			yield return new WaitForSeconds(simulationStartTime-Time.time);
			foreach (var navigator in uavsToNavigatorsDictionary.Values)
			{
				navigator.StartNavigation();
			}
		}
		
		private void GetReferencesFromGameManager()
		{
			_navigationSettings= GameManager.Instance.settingsDatabase.navigationSettings;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
		}
		private void OnDestroy() => UnsubscribeFromChannels();

		private void SubscribeToChannels()
		{
			if (_uavReroutedEventChannel != null) _uavReroutedEventChannel.Subscribe(RerouteUav);
			if (_uavDestroyedEventChannel != null) _uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			if (_uavConditionChangedEventChannel != null) _uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
		}
		private void UnsubscribeFromChannels()
		{
			if (_uavReroutedEventChannel != null) _uavReroutedEventChannel.Unsubscribe(RerouteUav);
			if (_uavDestroyedEventChannel != null) _uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			if (_uavConditionChangedEventChannel != null) _uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);

		}
	}
}