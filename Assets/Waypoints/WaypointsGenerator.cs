using System;
using System.Collections.Generic;
using System.Linq;
using IOHandlers;
using IOHandlers.Records;
using Unity.VisualScripting;
using UnityEngine;

namespace Waypoints
{
    public class WaypointsGenerator:MonoBehaviour
    {
        public int gridLength=4;
        public int gridWidth=4;
        [Serialize] public GameObject waypointPrefab=null;
        [NonSerialized] private GameObject _waypointsContainer;
        [SerializeField] private Transform waypointsPlane;
        float gridXDim;// TODO:LowPriority consider moving to inside the generator function
        float gridZDim;// TODO:LowPriority consider moving to inside the generator function

        private void Awake()
        {
            _waypointsContainer = gameObject.GetComponent<WaypointsManager>().waypointsContainer;
        }

        public void GenerateWaypointsUniformOverPlane(int numOfWaypoints, int numOfCols, int numOfRows)
        {
            // moving the waypoint container's position to align with the plane
            _waypointsContainer.transform.position = waypointsPlane.position;
            
            var planeScale = waypointsPlane.localScale;
            var planeMesh = waypointsPlane.GetComponent<MeshFilter>().mesh;
            var waypointMesh = waypointPrefab.GetComponent<MeshFilter>().sharedMesh;
        
            gridXDim = planeMesh.bounds.size.x * planeScale.x-waypointMesh.bounds.size.x;//the mesh dimension - the size of our object
            gridZDim = planeMesh.bounds.size.z * planeScale.z-waypointMesh.bounds.size.z;//the mesh dimension - the size of our object
            var waypointElevation = waypointMesh.bounds.size.y/2;

            for (int z=0; z<numOfCols; ++z)
            {
                for (int x=0; x<numOfRows; ++x)
                {
                    var id = z * numOfCols + x;
                    var xpos = -gridXDim/2 + x * ( gridXDim) / (gridLength-1);
                    var zpos=-gridZDim/2 + z * ( gridZDim) / (gridWidth-1);
                    
                    GenerateWaypoint(id, xpos, waypointElevation, zpos);
                }
            }
        }


        public void GenerateWaypointsFromRecords(List<WaypointRecord> waypointsRecords)
        {
            HandleNullValues(waypointsRecords);
            foreach (var waypointRecord in waypointsRecords)
            {
                GenerateWaypoint(waypointRecord.Id??=0,waypointRecord.Position.X??=0, waypointRecord.Position.Y??=0,waypointRecord.Position.Z??=0);
            }
        }

        private void HandleNullValues(List<WaypointRecord> waypointsRecords)
        {
            //if any waypoint has a null id, we need to assign it a new id.
            //we get the highest id in the list through linq, if it is null (no ids in the list, we set the max id to 0)
            var maxId = waypointsRecords.Max(x => x.Id) ?? 0;
            //we assign a new id to each waypoint that has a null id
            //aslo we check if position y is null, if it is, we set it to 0
            foreach (var waypointRecord in waypointsRecords)
            {
                waypointRecord.Id ??= maxId + 1;
                waypointRecord.Position.Y ??= 0;
            }
            //todo handle null values in positions x,z (this should not occur, so we should throw an error and a popup that states what the error is)
            
        }

        private void GenerateWaypoint(int id, float x, float y, float z)
        {
            var  waypointGameObject = Instantiate(waypointPrefab,  _waypointsContainer.transform);
            waypointGameObject.transform.localPosition = new Vector3(x, y, z);
            var waypoint= waypointGameObject.GetComponent<Waypoint>();
            waypoint.Id = id;
        }
    }
}