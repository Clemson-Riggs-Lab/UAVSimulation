using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using UnityEngine;
using VisualObjects;

namespace WayPoints
{
    public class WayPoint : MonoBehaviour
    {
        [NonSerialized] public int Id;
        [NonSerialized] public Transform Transform ;
        [NonSerialized] public List<VisualObject> VisualObjects = new List<VisualObject>();
        [NonSerialized] public bool HasTargets = false;
        
        [SerializeField] public ObjectEventChannelSO wayPointCreatedChannel= default;
        [SerializeField] public ObjectEventChannelSO wayPointDestroyedChannel= default;
      
        //copied from EasyWaypointSystem: probably dont need most of it
        // Debug visualization options
        public Color color;				// Waypoint gizmo color
        public float radius = 0.25f;   	// Waypoint gizmo size
        public string iconName; 		// Waypoint gizmo icon filename 
        
        
        public WayPoint(int id=999, bool hasTargets=false)
        {
            this.Id = id;
            this.HasTargets = hasTargets;
        }

        void Start()
        {
            Transform = gameObject.transform;
            if(wayPointCreatedChannel != null)
                wayPointCreatedChannel.RaiseEvent(this);
        }
        
        private void OnDisable()
        {
            if(wayPointDestroyedChannel != null)
                wayPointDestroyedChannel.RaiseEvent(this);
        }
    
        //copied from EasyWaypointSystem: probably dont need most of it
        //=============================================================================================================
        // Draw debug visualization
        void OnDrawGizmos () 
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);

            if (iconName !="")
            {
                var position = transform.position;
                Gizmos.DrawIcon (new Vector3(position.x, position.y+radius*1.5f, position.z), iconName, true);
            }
        }

        //----------------------------------------------------------------------------------
        public void Initialize(int id, Vector3 position)
        {
            this.Id = id;
            Transform= gameObject.transform;
            transform.position = position;
            gameObject.name="WayPoint "+id;
        }
    }
}


