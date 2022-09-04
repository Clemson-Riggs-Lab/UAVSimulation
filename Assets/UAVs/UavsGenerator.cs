using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;

namespace UAVs
{
    public class UavsGenerator : MonoBehaviour
    {
  
         private GameObject _uavPrefab;
         private GameObject _uavsContainer;
         private GameObject _wayPointsContainer;
         private WayPointsManager _wayPointsManager;
         private UavsManager _uavsManager;
         

        private void Start()
        {
            GetSettingsFromGameManager();
            AssertReferencesNotNull();
            _uavsContainer.transform.position = _wayPointsContainer.transform.position;//centering both on top of each other to avoid any offset due to local positioning
        }
        
        private void GetSettingsFromGameManager()
        {
            _wayPointsManager =GameManager.Instance.wayPointsManager;
            _uavsManager = GameManager.Instance.uavsManager;
            _uavsContainer = GameManager.Instance.uavsContainer;
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;
            _uavPrefab = GameManager.Instance.prefabsDatabase.uavPrefab;
        }
        private void AssertReferencesNotNull()
        {
            AssertionHelper.AssertPrefabReferenceObtainedFromPrefabsManager( _uavPrefab,"uav Prefab",this,gameObject);
            AssertionHelper.AssertObjectReferenceObtainedFromGameManager( _wayPointsManager,this,gameObject);
            AssertionHelper.AssertObjectReferenceObtainedFromGameManager( _uavsManager,this,gameObject);
            AssertionHelper.AssertObjectReferenceObtainedFromGameManager( _uavsContainer,this,gameObject);
            AssertionHelper.AssertObjectReferenceObtainedFromGameManager( _wayPointsContainer,this,gameObject);
        }
        
        public void GenerateOneUAVOnEachWayPoint()
        {
            var idIterator= 0;
            foreach (var wayPoint in _wayPointsManager.wayPoints)
            {
                GenerateUav(idIterator, wayPoint);
                idIterator++;
            }
        }
        public void GenerateUavsFromRecords(List<UavRecord> uavsRecords)
        {
            uavsRecords= UavRecord.HandleNullValues(uavsRecords);
            
            foreach (var uavRecord in uavsRecords)
            {
                var wayPoint = _wayPointsManager.wayPoints.FirstOrDefault(w => w.Id == uavRecord.StartingWayPointId);
                if (wayPoint != null)
                { 
                    var id = uavRecord.Id ??= 0;
                    var enabledOnStart= uavRecord.EnabledOnStart ??= true;
                    GenerateUav(id, wayPoint, enabledOnStart);
                }
                else
                {
                    Debug.LogError("WayPoint with id " + uavRecord.StartingWayPointId + " not found");
                }
            }
        }
        
        private void GenerateUav(int id, WayPoint wayPoint, bool enabledOnStart=true)
        {
            var  uav = Instantiate(_uavPrefab,  _uavsContainer.transform).GetComponent<Uav>();
            uav.Initialize(id, wayPoint,enabledOnStart);
        }
   
    }
}
