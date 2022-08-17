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

namespace WayPoints
{
    //TODO change so that it listens to wayPoint creation event and automatically adds it to the list
    public class WayPointsManager : MonoBehaviour
    {
        [SerializeField] public GameObject wayPointsContainer;
        [SerializeField] public WayPointsGenerator wayPointsGenerator;
        
        [SerializeField] private ObjectEventChannelSO wayPointCreatedChannel;
        [SerializeField] private ObjectEventChannelSO wayPointDisabledChannel;
        
        [DoNotSerialize] public List<WayPoint> wayPoints = new List<WayPoint>();
        

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(wayPointsContainer, this, this.gameObject);
            MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(wayPointsGenerator,this,this.gameObject);
        }
        private void OnEnable()
        {
            if(wayPointCreatedChannel != null)
                wayPointCreatedChannel.Subscribe(OnWayPointCreated);// subscribing to get each wayPoint that is created 
            if(wayPointDisabledChannel != null)
                wayPointDisabledChannel.Subscribe(OnWayPointDisabled);
        }


        private void Awake()
        {
            if (wayPointsGenerator != null) return; //if wayPointsGenerator is already set, don't do anything
           else
            {
                Debug.LogError("WayPoints Generator not found in the game object, couldn't continue program", this.gameObject);
                PlatformDependentScriptsHelper.Quit();
            }
        }

        private void Start()
        {
            wayPointsContainer= GameManager.Instance.wayPointsContainer;
        }

        public bool TryGetWayPoint(int wayPointID, out WayPoint wayPoint)
        {
            // check if wayPoints list contains item with given id
            wayPoint = wayPoints.FirstOrDefault(w => w.Id == wayPointID);
            if (wayPoint == null)
            {
                Debug.LogError("WayPoint with id " + wayPointID + " not found", this.gameObject);
                return false;
            }
            return true;
        }
        
        public void GenerateWayPoints()
        {
            ClearWayPoints(); //clear placeholders
            wayPointsGenerator.GenerateWayPointsUniformOverPlane(16,4,4);
        }
        
        public void GenerateWayPoints(List<WayPointRecord> wayPointsRecords)
        {
            ClearWayPoints(); //clear placeholders
            wayPointsGenerator.GenerateWayPointsFromRecords(wayPointsRecords); //generate wayPoints from records
        } 
        
        
        private void ClearWayPoints()
        {
            foreach (var wayPoint in wayPoints)
            {
                Destroy(wayPoint.gameObject);
            }
            wayPoints.Clear();
        }
        
        private void OnWayPointCreated(object wayPoint)
        {
            wayPoints.Add((WayPoint)wayPoint);
            Debug.Log("WayPoint created, call from wayPoints manager");
        }

        
        private void OnWayPointDisabled(object wayPoint)
        {
            wayPoints.Remove((WayPoint)wayPoint);
        }

      
    }
}

