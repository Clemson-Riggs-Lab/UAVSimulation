using System;
using System.Collections.Generic;
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

        private void Start()
        {
        }

        public List<Waypoint> GenerateWaypointsUniformOverPlane(int numOfWaypoints, int numOfCols, int numOfRows)
        {
            // moving the waypoint container's position to align with the plane
            _waypointsContainer.transform.position = waypointsPlane.position;
            
            var planeScale = waypointsPlane.localScale;
            var planeMesh = waypointsPlane.GetComponent<MeshFilter>().mesh;
            var waypointMesh = waypointPrefab.GetComponent<MeshFilter>().sharedMesh;
        
            gridXDim = planeMesh.bounds.size.x * planeScale.x-waypointMesh.bounds.size.x;//the mesh dimension - the size of our object
            gridZDim = planeMesh.bounds.size.z * planeScale.z-waypointMesh.bounds.size.z;//the mesh dimension - the size of our object
            var waypointElevation = waypointMesh.bounds.size.y/2;
            var waypointsList = new List<Waypoint>();
        
            for (int z=0; z<numOfCols; ++z)
            {
                for (int x=0; x<numOfRows; ++x)
                {
                    var id = z * numOfCols + x;
                    var xpos = -gridXDim/2 + x * ( gridXDim) / (gridLength-1);
                    var zpos=-gridZDim/2 + z * ( gridZDim) / (gridWidth-1); 
                
                    var waypointGameObject = GenerateWaypoint(id, xpos, waypointElevation, zpos);
                    var waypoint = waypointGameObject.GetComponent<Waypoint>();
                    waypointsList.Add(waypoint);
                }
            }
            return waypointsList;
        }
    
        public List<Waypoint> GenerateWaypointsUniformOverGrid(int numOfWaypoints, int numOfCols, int numOfRows)
        {
            var waypointsList = new List<Waypoint>();
        
            for (var z=0; z<numOfCols; ++z)
            {
                for (var x=0; x<numOfRows; ++x)
                {
                    var id = z * numOfCols + x;
                    var waypoint = GenerateWaypoint(id, x*gridLength, 0, z*gridWidth);
                    waypointsList.Add(waypoint);
                }
            }
            return waypointsList;
        }

        public Waypoint GenerateWaypoint(int id, float x, float y, float z)
        {
            var  waypointGameObject = Instantiate(waypointPrefab,  _waypointsContainer.transform);
            waypointGameObject.transform.localPosition = new Vector3(x, y, z);
            var waypoint= waypointGameObject.GetComponent<Waypoint>();
            waypoint.ID = id;
        
            return waypoint;    
        }
    }
}