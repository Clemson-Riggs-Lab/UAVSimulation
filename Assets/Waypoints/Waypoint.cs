using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using UnityEngine;
using VisualObjects;

namespace Waypoints
{
    public class Waypoint : MonoBehaviour
    {
        [NonSerialized] public int Id;
        [NonSerialized] public Transform Transform ;
        [NonSerialized] public List<VisualObject> VisualObjects = new List<VisualObject>();
        [NonSerialized] public bool HasTargets = false;
        
        [SerializeField] public ObjectEventChannelSO waypointCreatedChannel= default;
        [SerializeField] public ObjectEventChannelSO waypointDestroyedChannel= default;
        public Waypoint(int id=999, bool hasTargets=false)
        {
            this.Id = id;
            this.HasTargets = hasTargets;
        }

        void Start()
        {
            Transform = gameObject.transform;
            if(waypointCreatedChannel != null)
                waypointCreatedChannel.RaiseEvent(this);
        }
        
        private void OnDisable()
        {
            if(waypointDestroyedChannel != null)
                waypointDestroyedChannel.RaiseEvent(this);
        }
    
    }
}


