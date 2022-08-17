using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using UAVs.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;

namespace UAVs
{
	public class Uav : MonoBehaviour
	{
		private int _id = 999;
		[NonSerialized] public string codeName;
		[NonSerialized] public string abbrvName;
		[SerializeField] public ObjectEventChannelSO uavCreatedChannel;
		[SerializeField] public ObjectEventChannelSO uavDestroyedChannel;

		[NonSerialized] public WayPoint startingWaypoint;
		
		[NonSerialized] public bool isVisuallyEnabled;
		private Renderer _renderer;

		public int ID
		{
			get => _id;
			set => SetIDandNames(value);
		}

		private void OnEnable()
		{
			_renderer = GetComponent<Renderer>();
		}
		

		public void Initialize(int id, WayPoint wayPoint)
		{
			gameObject.name = "UAV " + id;
			this.ID = id;
			
			startingWaypoint = wayPoint;
			transform.position = startingWaypoint.transform.position; // TODO: check if just position is enough or should I use local position
			
			this.isVisuallyEnabled = false;
			
			if(uavCreatedChannel != null) 
				uavCreatedChannel.RaiseEvent(this);
			
		}


		private void SetIDandNames(int value)
		{
			_id = value;
			abbrvName = NatoAlphabetConverter.IntToLetters(value);
			codeName = NatoAlphabetConverter.LettersToName(abbrvName);
			
		}
		
		
		private void OnDisable()
		{
			if(uavDestroyedChannel != null)
				uavDestroyedChannel.RaiseEvent(this);
		}

		public void SetUavRecord(UavRecord uavRecord)
		{
			this.isVisuallyEnabled= uavRecord.EnabledOnStart??false;
		}

		public void SetUavVisuallyEnabled(bool p0)
		{
			_renderer.enabled = p0;
		}
	}
}