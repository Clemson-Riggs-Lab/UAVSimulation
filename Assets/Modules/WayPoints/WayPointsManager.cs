using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using Modules.WayPoints;
using ScriptableObjects.EventChannels;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;
using WayPoints.Settings.ScriptableObjects;
using static HelperScripts.Enums.InputRecordsSource;

namespace WayPoints
{
    public class WayPointsManager : MonoBehaviour
    {
         private GameObject _wayPointsContainer;
         private WayPointsGenerator _wayPointsGenerator;
         private WaypointEventChannelSO _wayPointCreatedEventChannel;
         private WaypointEventChannelSO _wayPointDisabledEventChannel;
         private WaypointSettingsSO _waypointSettings;
         [NonSerialized] public List<WayPoint> wayPoints = new ();
         public void Initialize()
        { 
            GetReferencesFromGameManager();
            SubscribeToChannels();
            
            _wayPointsGenerator = gameObject.AddComponent<WayPointsGenerator>();
            _wayPointsGenerator.Initialize();
            GenerateWayPoints();
        }

        private void GetReferencesFromGameManager()
        {
            _wayPointsContainer= GameManager.Instance.wayPointsContainer;
            _waypointSettings = GameManager.Instance.settingsDatabase.waypointSettings;
            _wayPointCreatedEventChannel = GameManager.Instance.channelsDatabase.wayPointCreatedEventChannel;
            _wayPointDisabledEventChannel = GameManager.Instance.channelsDatabase.wayPointDisabledEventChannel;
        }


        public bool TryGetWayPoint(int wayPointID, out WayPoint wayPoint)
        {
            // check if wayPoints list contains item with given id
            wayPoint = wayPoints.FirstOrDefault(w => w.id == wayPointID);
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
            _wayPointsGenerator.GenerateWayPointsOverGrid(_waypointSettings.waypointHeightFromTerrain, _waypointSettings.numberOfWaypoints);
            
        } 
        
        
        private void ClearWayPoints()
        {
            foreach (var wayPoint in wayPoints)
            {
                Destroy(wayPoint.gameObject);
            }
            wayPoints.Clear();
        }
        
        private void OnWayPointCreated(WayPoint wayPoint)
        {
            wayPoints.Add(wayPoint);
        }
        
        private void OnWayPointDisabled(WayPoint wayPoint)
        {
            wayPoints.Remove(wayPoint);
        }

        
        private void OnDisable()
        {
            UnsubscribeFromChannels();
        }
        
        
        private void SubscribeToChannels()
        {
            if(_wayPointCreatedEventChannel != null)
                _wayPointCreatedEventChannel.Subscribe(OnWayPointCreated);
            if(_wayPointDisabledEventChannel != null)
                _wayPointDisabledEventChannel.Subscribe(OnWayPointDisabled);
        }
        
        private void UnsubscribeFromChannels()
        {
            if(_wayPointCreatedEventChannel != null)
                _wayPointCreatedEventChannel.Unsubscribe(OnWayPointCreated);
            if(_wayPointDisabledEventChannel != null)
                _wayPointDisabledEventChannel.Unsubscribe(OnWayPointDisabled);
        }

    }
}

