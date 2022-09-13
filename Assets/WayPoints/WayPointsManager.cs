using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using ScriptableObjects.EventChannels;
using ScriptableObjects.Waypoints;
using Unity.VisualScripting;
using UnityEngine;

namespace WayPoints
{
    public class WayPointsManager : MonoBehaviour
    {
         private GameObject wayPointsContainer;
         private WayPointsGenerator wayPointsGenerator;
         private ObjectEventChannelSO wayPointCreatedEventChannel;
         private ObjectEventChannelSO wayPointDisabledEventChannel;
         private WaypointSettingsSO waypointSettings;
         [DoNotSerialize] public List<WayPoint> wayPoints = new ();
         
        public void Initialize()
        { 
            GetReferencesFromGameManager();
            wayPointsGenerator = gameObject.AddComponent<WayPointsGenerator>();
            wayPointsGenerator.Initialize();
            SubscribeToChannels();
        }

        private void GetReferencesFromGameManager()
        {
            wayPointsContainer= GameManager.Instance.wayPointsContainer;
            waypointSettings = GameManager.Instance.settingsDatabase.waypointSettings;
            wayPointCreatedEventChannel = GameManager.Instance.channelsDatabase.wayPointCreatedEventChannel;
            wayPointDisabledEventChannel = GameManager.Instance.channelsDatabase.wayPointDisabledEventChannel;
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
            
            switch(waypointSettings.waypointsRecordsSource)
            {
                case Enums.InputRecordsSource.FromInputFile:
                    wayPointsGenerator.GenerateWayPointsFromRecords(GameManager.Instance.inputRecordsDatabase.WayPointsRecords);
                    break;
                case Enums.InputRecordsSource.FromDefaultRecords:
                    wayPointsGenerator.GenerateWayPointsFromRecords(DefaultRecordsCreator.GetDefaultWayPointsRecords());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        }
        
        private void OnWayPointDisabled(object wayPoint)
        {
            wayPoints.Remove((WayPoint)wayPoint);
        }

        
        private void OnDisable()
        {
            UnsubscribeFromChannels();
        }
        
        
        private void SubscribeToChannels()
        {
            if(wayPointCreatedEventChannel != null)
                wayPointCreatedEventChannel.Subscribe(OnWayPointCreated);
            if(wayPointDisabledEventChannel != null)
                wayPointDisabledEventChannel.Subscribe(OnWayPointDisabled);
        }
        
        private void UnsubscribeFromChannels()
        {
            if(wayPointCreatedEventChannel != null)
                wayPointCreatedEventChannel.Unsubscribe(OnWayPointCreated);
            if(wayPointDisabledEventChannel != null)
                wayPointDisabledEventChannel.Unsubscribe(OnWayPointDisabled);
        }

    }
}

