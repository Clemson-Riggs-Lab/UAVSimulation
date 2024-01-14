using System;
using System.Collections.Generic;
using HelperScripts;
using ScriptableObjects.EventChannels;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;

namespace WayPoints
{
    public class WayPoint : MonoBehaviour
    {
        [NonSerialized] public int id;
        [SerializeField] public WaypointEventChannelSO wayPointCreatedChannel= default;
        [SerializeField] public WaypointEventChannelSO wayPointDisabledChannel= default;
        
        public void Initialize(int id, Vector3 position)
        {
            GetReferencesFromGameManager();
            this.id = id;
            transform.position = position;
            gameObject.name="WayPoint "+id;
            this.transform.SetLayerRecursively(LayerMask.NameToLayer("Waypoints"));
            
            if(wayPointCreatedChannel != null)
                wayPointCreatedChannel.RaiseEvent(this);
        }

        private void GetReferencesFromGameManager()
        {
            wayPointCreatedChannel = GameManager.Instance.channelsDatabase.wayPointCreatedEventChannel;
            wayPointDisabledChannel = GameManager.Instance.channelsDatabase.wayPointDisabledEventChannel;
        }

        private void OnDisable()
        {
            if(wayPointDisabledChannel != null)
                wayPointDisabledChannel.RaiseEvent(this);
        }
    }
}


