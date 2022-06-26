using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using Unity.VisualScripting;
using UnityEngine;

namespace Waypoints
{
    //TODO change so that it listens to waypoint creation event and automatically adds it to the list
    public class WaypointsManager : MonoBehaviour
    {
        [SerializeField] public GameObject waypointsContainer;
        [SerializeField] public WaypointsGenerator waypointsGenerator;
        
        [SerializeField] private ObjectEventChannelSO waypointCreatedChannel;
        [SerializeField] private ObjectEventChannelSO waypointDisabledChannel;
        
        [DoNotSerialize] public List<Waypoint> waypoints = new List<Waypoint>();
        

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(waypointsContainer, this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(waypointsGenerator,this,this.gameObject);
        }
        private void OnEnable()
        {
            if(waypointCreatedChannel != null)
                waypointCreatedChannel.Subscribe(OnWaypointCreated);// subscribing to get each waypoint that is created 
            if(waypointDisabledChannel != null)
                waypointDisabledChannel.Subscribe(OnWaypointDisabled);
        }


        private void Awake()
        {
            if (waypointsGenerator != null) return; //if waypointsGenerator is already set, don't do anything
           else
            {
                Debug.LogError("Waypoints Generator not found in the game object, couldn't continue program", this.gameObject);
                PlatformDependentScriptsHelper.Quit();
            }
        }
        
        public bool TryGetWaypoint(int waypointID, out Waypoint waypoint)
        {
            // check if waypoints list contains item with given id
            waypoint = waypoints.FirstOrDefault(w => w.Id == waypointID);
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
            waypointsGenerator.GenerateWaypointsUniformOverPlane(16,4,4);
        }
        
        public void GenerateWaypoints(List<WaypointRecord> waypointsRecords)
        {
            ClearWaypoints(); //clear placeholders
            waypointsGenerator.GenerateWaypointsFromRecords(waypointsRecords); //generate waypoints from records
        } 
        
        
        private void ClearWaypoints()
        {
            foreach (var waypoint in waypoints)
            {
                Destroy(waypoint.gameObject);
            }
            waypoints.Clear();
        }
        
        private void OnWaypointCreated(object waypoint)
        {
            waypoints.Add((Waypoint)waypoint);
            Debug.Log("Waypoint created, call from waypoints manager");
        }

        
        private void OnWaypointDisabled(object waypoint)
        {
            waypoints.Remove((Waypoint)waypoint);
        }

      
    }
}

