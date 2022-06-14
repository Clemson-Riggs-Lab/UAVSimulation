using System;
using System.Collections.Generic;
using System.Linq;
using Helper_Scripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Waypoints
{
    public class WaypointsManager : MonoBehaviour
    {
        [SerializeField] public GameObject waypointsContainer;
        [SerializeField] public WaypointsGenerator waypointsGenerator;
        
        [DoNotSerialize] public List<Waypoint> waypoints = new List<Waypoint>();
        
        //events
        public delegate void Notify();
        public event Notify  OnGeneratedWaypointsNotify;

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(waypointsContainer, this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(waypointsGenerator,this,this.gameObject);
        }

        private void Awake()
        {
            if (waypointsGenerator != null) return; //if waypointsGenerator is already set, don't do anything
            if (!this.gameObject.TryGetComponent(out waypointsGenerator)) //couldn't find in the game object
            {
                Debug.LogError("Waypoints Generator not found in the game object, couldn't continue program", this.gameObject);
                PlatformDependentScriptsHelper.Quit();
            }
        }

        private void Start()
        {
            waypoints= GetAllWaypoints();
        }
        
        private List<Waypoint> GetAllWaypoints()
        {
            return waypointsContainer.GetComponentsInChildren<Waypoint>().ToList();
        }

        public bool TryGetWaypoint(int waypointID, out Waypoint waypoint)
        {
            // check if waypoints list contains item with given id
            waypoint = waypoints.FirstOrDefault(w => w.ID == waypointID);
            if (waypoint == null)
            {
                Debug.LogError("Waypoint with id " + waypointID + " not found", this.gameObject);
                return false;
            }
            return true;
        }
        
        public void GenerateWaypoints()
        {
            ClearWaypoints(); //clear placeholders
            waypoints = waypointsGenerator.GenerateWaypointsUniformOverPlane(16,4,4);
            OnGeneratedWaypointsNotify?.Invoke();
        }
        
        
        
        
        private void ClearWaypoints()
        {
            foreach (var waypoint in waypoints)
            {
                Destroy(waypoint.gameObject);
            }
            waypoints.Clear();
        }

      
    }
}

