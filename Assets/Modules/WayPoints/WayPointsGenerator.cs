using UnityEngine;
using WayPoints;

namespace Modules.WayPoints
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

        
        public void GenerateWayPointsOverGrid(int heightFromTerrain, int numberOfWaypoints)
        {
            var terrain = _terrainContainer.GetComponentInChildren<Terrain>();

            // Get the size of the terrain
            float terrainWidth = terrain.terrainData.size.x;
            float terrainLength = terrain.terrainData.size.z;

            // Calculate the distance between waypoints based on the number of waypoints and the terrain size
            float distanceBetweenWaypoints = Mathf.Sqrt((terrainWidth * terrainLength) / numberOfWaypoints);

            // Generate the waypoints uniformly distributed around the grid
            for (int i = 0; i < numberOfWaypoints; i++)
            {
                // Calculate the x and z coordinates of the current waypoint
                float x = (i % Mathf.RoundToInt(terrainWidth / distanceBetweenWaypoints)) * distanceBetweenWaypoints+ distanceBetweenWaypoints/2;
                float z = Mathf.FloorToInt(i / (terrainLength / distanceBetweenWaypoints)) * distanceBetweenWaypoints + distanceBetweenWaypoints/2;

                // Get the height of the terrain at the current waypoint position
                float y = terrain.SampleHeight(new Vector3(x, 0, z));

                // Generate the waypoint at the calculated position
                GenerateWayPoint(i, new Vector3(x, y + heightFromTerrain, z));
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


    }
}