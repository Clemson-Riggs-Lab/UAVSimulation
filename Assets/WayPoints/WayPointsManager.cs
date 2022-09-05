using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using ScriptableObjects.EventChannels;
using Unity.VisualScripting;
using UnityEngine;

namespace WayPoints
{
    public class WayPointsManager : MonoBehaviour
    {
        [SerializeField] public GameObject wayPointsContainer;
        [SerializeField] public WayPointsGenerator wayPointsGenerator;
        
        [SerializeField] private ObjectEventChannelSO wayPointCreatedEventChannel;
        [SerializeField] private ObjectEventChannelSO wayPointDisabledEventChannel;
        
        [DoNotSerialize] public List<WayPoint> wayPoints = new ();
        

        private void OnValidate()
        {
            AssertionHelper.AssertComponentReferencedInEditor(wayPointsContainer, this, this.gameObject);
            AssertionHelper.CheckIfReferenceExistsOrComponentExistsInGameObject(wayPointsGenerator,this,this.gameObject);
        }
        public void Initialize()
        { 
            GetSettingsFromGameManager(); 
            SubscribeToChannels();
        }

        private void GetSettingsFromGameManager()
        {
            wayPointCreatedEventChannel = GameManager.Instance.channelsDatabase.wayPointCreatedEventChannel;
            wayPointDisabledEventChannel = GameManager.Instance.channelsDatabase.wayPointDisabledEventChannel;
        }


        private void Awake()
        {
            if (wayPointsGenerator != null) return; //if wayPointsGenerator is already set, don't do anything
           else
            {
                Debug.LogError("WayPoints Generator not found in the game object, couldn't continue program", this.gameObject);
                return;
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
        }
        
        private void OnWayPointDisabled(object wayPoint)
        {
            wayPoints.Remove((WayPoint)wayPoint);
        }

        
        private void OnDisable()
        {
            UnsubscribeFromChannels();
        }
        
        private void OnDestroy()
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

