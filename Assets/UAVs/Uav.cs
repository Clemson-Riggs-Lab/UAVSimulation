using System;
using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs;
using ScriptableObjects.UAVs.Navigation;
using TMPro;
using UAVs.Sub_Modules.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;

namespace UAVs
{
	public class Uav : MonoBehaviour
	{
		private UavSettingsDatabaseSO uavSettingsDatabase;
		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;
		
		[DoNotSerialize]public string uavName;
		[DoNotSerialize]public int id;
		[DoNotSerialize]public Path currentPath;
		[DoNotSerialize]public Color uavColor;
		[NonSerialized] public WayPoint startingWaypoint;
		
		public TextMeshPro label;
		public GameObject uavBody;
		public Renderer uavRenderer;
		
		
		private void OnEnable()
		{
			GetReferencesFromGameManager();
		}
		
		
		private void GetReferencesFromGameManager()
		{
			uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			uavSettingsDatabase= GameManager.Instance.settingsDatabase.uavSettingsDatabase;
		}

		public void Initialize(int id, WayPoint wayPoint,bool enabledOnStart)
		{
			this.id = id;
			SetUavName();
			gameObject.name = uavName;
			uavColor=ColorHelper.GetUniqueColorFromId(id);
			
			transform.SetLayerRecursively(LayerMask.NameToLayer("UAV"+id));//set layer to UAV1, UAV2, UAV3, etc.. including all children, based on UavName
			
			startingWaypoint = wayPoint;
			transform.position = startingWaypoint.transform.position;
			
			if(uavSettingsDatabase.uavGeneralSettings.colorUavLikePath)
				uavRenderer.material.color = uavColor;//set material color based on uavcolor
			
			//set label name based on name
			label.text = uavName;
			label.color = uavColor;
			
			if(uavCreatedEventChannel != null) 
				uavCreatedEventChannel.RaiseEvent(this);
			
		}

		private void SetUavName()
		{
			uavName = uavSettingsDatabase.uavGeneralSettings.namingScheme switch
			{
				UavSettingsDatabaseSO.UavNamingScheme.UavAndNumber => "UAV " + id,
				UavSettingsDatabaseSO.UavNamingScheme.UavAndNumberOffsetZero => "UAV " + (id + 1),
				UavSettingsDatabaseSO.UavNamingScheme.HashtagNumber => "# " + (id),
				UavSettingsDatabaseSO.UavNamingScheme.HashtagNumberOffsetZero =>  "# " + (id + 1),
				UavSettingsDatabaseSO.UavNamingScheme.Letter =>  NatoAlphabetConverter.IntToLetters(id),
				UavSettingsDatabaseSO.UavNamingScheme.NatoName =>  NatoAlphabetConverter.LettersToName(NatoAlphabetConverter.IntToLetters(id))
				,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private void OnDisable()
		{
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.RaiseEvent(this);
		}
		
	}
}