using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using Unity.VisualScripting;
using UnityEngine;

namespace WayPoints
{
    public class WayPointsGenerator:MonoBehaviour
    {
        private GameObject _wayPointPrefab=null;
        private GameObject _wayPointsContainer;
        private GameObject _terrainContainer;

        public void Initialize()
        { 
            GetReferencesFromGameManager();
        }

        private void GetReferencesFromGameManager()
        { 
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;
            _terrainContainer = GameManager.Instance.terrainContainer;
            _wayPointPrefab = GameManager.Instance.prefabsDatabase.waypointPrefab;
        }
        

        public void GenerateWayPointsFromRecords(List<WayPointRecord> wayPointsRecords)
        {
            HandleNullValues(wayPointsRecords);
            foreach (var wayPointRecord in wayPointsRecords)
            {
                var position = new Vector3(wayPointRecord.Position.X??=0, wayPointRecord.Position.Y??=0,wayPointRecord.Position.Z??=0);
                GenerateWayPoint(wayPointRecord.Id??=0,position);
            }
        }
        private void GenerateWayPoint(int id,Vector3 position)
        {
            var  wayPointGameObject = Instantiate(_wayPointPrefab,  _wayPointsContainer.transform);
            wayPointGameObject.transform.localPosition = position;
            var wayPoint= wayPointGameObject.GetComponent<WayPoint>();
            wayPoint.Initialize(id,position);
            wayPoint.id = id;
        }
        
        
        private void HandleNullValues(List<WayPointRecord> wayPointsRecords)
        {
            //if any wayPoint has a null id, we need to assign it a new id.
            //we get the highest id in the list through linq, if it is null (no ids in the list, we set the max id to 0)
            var maxId = wayPointsRecords.Max(x => x.Id) ?? 0;
            //we assign a new id to each wayPoint that has a null id
            //aslo we check if position y is null, if it is, we set it to 0
            foreach (var wayPointRecord in wayPointsRecords)
            {
                wayPointRecord.Id ??= maxId + 1;
                wayPointRecord.Position.Y ??= 0;
            }
            //todo handle null values in positions x,z (this should not occur, so we should throw an error and a popup that states what the error is)
            
        }


    }
}