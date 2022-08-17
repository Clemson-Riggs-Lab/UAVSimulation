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
            _wayPointsManager =GameManager.Instance.wayPointsManager;
            _uavsManager = GameManager.Instance.uavsManager;
            _uavsContainer = GameManager.Instance.uavsContainer;
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;

            MyDebug.AssertObjectReferenceObtainedFromGameManager( _wayPointsManager,this,gameObject);
            MyDebug.AssertObjectReferenceObtainedFromGameManager( _uavsManager,this,gameObject);
            MyDebug.AssertObjectReferenceObtainedFromGameManager( _uavsContainer,this,gameObject);
            MyDebug.AssertObjectReferenceObtainedFromGameManager( _wayPointsContainer,this,gameObject);
            
            _uavPrefab = PrefabsManager.Instance.uavPrefab;
            MyDebug.AssertPrefabReferenceObtainedFromPrefabsManager( _uavPrefab,"uav Prefab",this,gameObject);
            
            _uavsContainer.transform.position = _wayPointsContainer.transform.position;//centering both on top of each other to avoid any offset due to local positioning
        }

        public List<Uav> GenerateOneUAVOnEachWayPoint()
        {
            var idIterator= 0;
            var uavs = new List<Uav>();
            
            foreach (var wayPoint in _wayPointsManager.wayPoints)
            {
                var uav = GenerateUav(idIterator, wayPoint);
                uavs.Add(uav);
                idIterator++;
            }

            return uavs;
        }
        public List<Uav> GenerateUavs(List<UavRecord> uavsRecords)
        {
            HandleNullValues(uavsRecords);
            var uavs = new List<Uav>();
            foreach (var uavRecord in uavsRecords)
            {
                //linq to get the wayPoint by id
                var wayPoint = _wayPointsManager.wayPoints.FirstOrDefault(w => w.Id == uavRecord.StartingWayPointId);
                if(wayPoint != null)
                {
                    var uav = GenerateUav(uavRecord.Id??=0, wayPoint);
                    uav.SetUavRecord(uavRecord);
                    uavs.Add(uav);
                }
                else
                {
                    Debug.LogError("WayPoint with id " + uavRecord.StartingWayPointId + " not found");
                }
            }
            return uavs;
        }

        private void HandleNullValues(List<UavRecord> uavsRecords)
        {
            var maxId = uavsRecords.Max(x => x.Id) ?? 0;
            foreach (var uavsRecord in uavsRecords)
            {
                uavsRecord.Id ??= maxId + 1;
            }
        }
        

        private Uav GenerateUav(int id, WayPoint wayPoint)
        {
            var  uavGO = Instantiate(_uavPrefab,  _uavsContainer.transform) as GameObject ;
            var uav= uavGO.GetComponent<Uav>();
            uav.Initialize(id, wayPoint);
            uav.ID = id;
        
            return uav;
        }

       
    }
}
