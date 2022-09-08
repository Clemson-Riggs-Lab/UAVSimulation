using System;
using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Navigation;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using WayPoints;

namespace UAVs
{
	public class Uav : MonoBehaviour
	{
		public int ID { get; private set; }
		public string AbbreviatedName =>NatoAlphabetConverter.IntToLetters(ID);
		public string CodeName => NatoAlphabetConverter.LettersToName(AbbreviatedName);

		public Path currentPath;

		 private UavEventChannelSO uavCreatedEventChannel;
		 private UavEventChannelSO uavDestroyedEventChannel;
		
		 
		[NonSerialized] public WayPoint startingWaypoint;
		
		[NonSerialized] public bool isVisuallyEnabled;
		private Renderer _renderer;

		
	
		private void OnEnable()
		{
			SetChannelsReferences();
			_renderer = GetComponent<Renderer>();
		}
		
		
		private void SetChannelsReferences()
		{
			uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
		}

		public void Initialize(int id, WayPoint wayPoint,bool enabledOnStart)
		{
			gameObject.name = "UAV " + id;
			this.ID = id;
			transform.SetLayerRecursively(LayerMask.NameToLayer("UAV"+id));//set layer to UAV1, UAV2, UAV3, etc.. including all children, based on ID
			startingWaypoint = wayPoint;
			transform.position = startingWaypoint.transform.position;
			
			this.isVisuallyEnabled = enabledOnStart;
			
			if(uavCreatedEventChannel != null) 
				uavCreatedEventChannel.RaiseEvent(this);
			
		}
		
		
		private void OnDisable()
		{
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.RaiseEvent(this);
		}

		public void SetUavVisuallyEnabled(bool p0)
		{
			_renderer.enabled = p0;
		}
	}
}