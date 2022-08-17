using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using Unity.VisualScripting;
using UnityEngine;

namespace WayPoints
{
    public class WayPointsGenerator:MonoBehaviour
    {
        public int gridLength=4;
        public int gridWidth=4;
        private GameObject _wayPointPrefab=null;
        private GameObject _wayPointsContainer;
        private GameObject _terrainContainer;
        float gridXDim;// TODO:LowPriority consider moving to inside the generator function
        float gridZDim;// TODO:LowPriority consider moving to inside the generator function
        
        private void Start()
        {
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;
            _terrainContainer = GameManager.Instance.terrainContainer;
            _wayPointPrefab = PrefabsManager.Instance.waypointPrefab;
            
            MyDebug.AssertObjectReferenceObtainedFromGameManager(_wayPointsContainer, this, gameObject);
            MyDebug.AssertObjectReferenceObtainedFromGameManager(_terrainContainer, this, gameObject);
            MyDebug.AssertPrefabReferenceObtainedFromPrefabsManager(_wayPointPrefab,"waypointPrefab", this, gameObject);
            
            
        }

        public void GenerateWayPointsUniformOverPlane(int numOfWayPoints, int numOfCols, int numOfRows)
        {
            
            // moving the wayPoint container's position to align with the plane
            _wayPointsContainer.transform.position = _terrainContainer.transform.position;
            var terrain = _terrainContainer.GetComponentInChildren<Terrain>();
            var wayPointMesh = _wayPointPrefab.GetComponent<MeshFilter>().sharedMesh;
        
            gridXDim =  terrain.terrainData.size.x;
            gridZDim =  terrain.terrainData.size.z ;
            var wayPointElevation = 100;//TODO change from fixed to dynamic height

            for (int z=0; z<numOfCols; ++z)
            {
                for (int x=0; x<numOfRows; ++x)
                {
                    var id = z * numOfCols + x;
                    var xpos =  (x+1) * ( gridXDim) / (gridLength+1);
                    var zpos= (z+1) * ( gridZDim) / (gridWidth+1);
                    var position = new Vector3(xpos, wayPointElevation, zpos);
                    GenerateWayPoint(id,position);
                }
            }
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

        private void GenerateWayPoint(int id,Vector3 position)
        {
            var  wayPointGameObject = Instantiate(_wayPointPrefab,  _wayPointsContainer.transform);
            wayPointGameObject.transform.localPosition = position;
            var wayPoint= wayPointGameObject.GetComponent<WayPoint>();
            wayPoint.Initialize(id,position);
            wayPoint.Id = id;
        }
    }
}