using System;
using System.Collections.Generic;
using HelperScripts;
using Modules.Navigation;
using ScriptableObjects.EventChannels;
using TMPro;
using UAVs.Channels.ScriptableObjects;
using UAVs.Settings.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;
using static HelperScripts.Enums;

namespace UAVs
{
	public class Uav : MonoBehaviour
	{
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavSettingsSO _uavSettings;
		
		[NonSerialized]public string uavName;
		[NonSerialized]public int id;
		[NonSerialized]public Path currentPath;
		[NonSerialized]public UavCondition uavCondition;
		[NonSerialized]public Color uavColor;
		[NonSerialized]public WayPoint startingWaypoint;
		
		public TextMeshPro label;
		public GameObject uavBody;
		public Renderer uavRenderer;
		public int dedicatedLayer;

		private void Awake()
		{
			GetReferencesFromGameManager();
		}

		private void GetReferencesFromGameManager()
		{
			_uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavSettings= GameManager.Instance.settingsDatabase.uavSettings;
		}

		public void Initialize(int id, WayPoint wayPoint)
		{
			this.id = id;
			SetUavName();
			gameObject.name = uavName;
			uavColor=ColorHelper.GetUniqueColorFromId(id);
			dedicatedLayer = UavsManager.GetEmptyLayer();
			transform.SetLayerRecursively(dedicatedLayer);
			startingWaypoint = wayPoint;
			transform.position = startingWaypoint.transform.position;
			
			if(_uavSettings.colorUavLikePath)
				uavRenderer.material.color = uavColor;//set material color based on uavcolor
			
			//set label name based on name
			label.text = uavName;
			label.color = uavColor;
			
			if(_uavCreatedEventChannel != null) 
				_uavCreatedEventChannel.RaiseEvent(this);
			
		}

		private void SetUavName()
		{
			uavName = _uavSettings.namingScheme switch
			{
				UavNamingScheme.UavAndNumber => "UAV " + id,
				UavNamingScheme.UavAndNumberOffsetZero => "UAV " + (id + 1),
				UavNamingScheme.HashtagNumber => "# " + (id),
				UavNamingScheme.HashtagNumberOffsetZero =>  "# " + (id + 1),
				UavNamingScheme.Letter =>  NatoAlphabetConverter.IntToLetters(id),
				UavNamingScheme.NatoName =>  NatoAlphabetConverter.LettersToName(NatoAlphabetConverter.IntToLetters(id)),
				UavNamingScheme.NatoNameWithRandomNumber => NatoAlphabetConverter.LettersToUniqueName(NatoAlphabetConverter.IntToLetters(id,true,true)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private void OnDisable()
		{
			if(_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.RaiseEvent(this);
		}

		public void SetVisibility(bool visibility)
		{
			uavBody.transform.SetLayerRecursively(visibility ? dedicatedLayer:LayerMask.NameToLayer("UAVHidden"));
			label.transform.SetLayerRecursively(visibility ? dedicatedLayer:LayerMask.NameToLayer("UAVHidden"));
			// the minimap camera is set to cull the UAVHidden layer, so the UAVs are not visible on the minimap if they are placed in the UAVHidden layer
		}

		public void SetCollisions(bool collisions)
		{
			//set tag to UAV or UAVNoCollisions based on collisions status
			// if tag is set to UAVNoCollisions, the NFZ will not register a collision since it only checks for UAV tag
			uavBody.gameObject.tag = collisions ? "UAV" : "UAVNoCollisions";
		}
	
	}
}