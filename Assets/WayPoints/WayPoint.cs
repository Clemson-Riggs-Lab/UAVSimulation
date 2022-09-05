using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using TargetDetection;
using UnityEngine;

namespace WayPoints
{
    public class WayPoint : MonoBehaviour
    {
        [NonSerialized] public int Id;
        [SerializeField] public ObjectEventChannelSO wayPointCreatedChannel= default;
        [SerializeField] public ObjectEventChannelSO wayPointDestroyedChannel= default;
        
        
        void Start()
        {
            if(wayPointCreatedChannel != null)
                wayPointCreatedChannel.RaiseEvent(this);
        }
        public void Initialize(int id, Vector3 position)
        {
            this.Id = id;
            transform.position = position;
            gameObject.name="WayPoint "+id;
        }
        private void OnDisable()
        {
            if(wayPointDestroyedChannel != null)
                wayPointDestroyedChannel.RaiseEvent(this);
        }
    }
}


