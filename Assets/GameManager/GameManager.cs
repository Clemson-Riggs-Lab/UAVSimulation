using System;
using Helper_Scripts;
using UAVs;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public GameObject waypointsContainer;
        [SerializeField] public GameObject uavsContainer;
        [SerializeField] public WaypointsManager waypointsManager;
        [SerializeField] public UavsManager uavsManager;

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(waypointsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);

        }

        private void Start()
        {
            waypointsManager = waypointsContainer.GetComponent<WaypointsManager>();
            uavsManager = uavsContainer.GetComponent<UavsManager>();
            Invoke(nameof(InitializeSimulation),2);
            
        }

        private void InitializeSimulation()
        {
            waypointsManager.GenerateWaypoints();
            uavsManager.GenerateUavs();
            _NavigationManager.GeneratePaths()
        }
    }
}