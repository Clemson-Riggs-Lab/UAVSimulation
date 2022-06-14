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
        [SerializeField] public PathsManager pathsManager;

        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(waypointsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(pathsManager,this,this.gameObject);
        }

        private void Start()
        {
            waypointsManager = waypointsContainer.GetComponent<WaypointsManager>();
            uavsManager = uavsContainer.GetComponent<UavsManager>();
            Invoke(nameof(InitializeSimulation),2);//start initialization of simulation after 2 seconds
            
        }

        private void InitializeSimulation()
        {
            waypointsManager.GenerateWaypoints();
            uavsManager.GenerateUavs();
            pathsManager.GeneratePaths(PathsManager.NavType.InSequence);
            uavsManager.NavigateAll();
        }
    }
}