using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
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
        [SerializeField] public JsonSerializerTest jsonSerializerTest;
        
        
        private bool _generateFromRecords = true;
        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(waypointsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(pathsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(jsonSerializerTest,this,this.gameObject);
        }


        private void Start()
        {
            waypointsManager = waypointsContainer.GetComponent<WaypointsManager>();
            uavsManager = uavsContainer.GetComponent<UavsManager>();
            StartCoroutine(InitializeSimulation());

        }

        private IEnumerator InitializeSimulation()
        {
            
            if (!_generateFromRecords)
            {
                waypointsManager.GenerateWaypoints();
               uavsManager.GenerateUavs();
                pathsManager.GeneratePaths(PathsManager.NavType.InSequence);
                uavsManager.NavigateAll();
            }
            else
            {
                
             waypointsManager.GenerateWaypoints(jsonSerializerTest.rootObject.WaypointsRecords);
             yield return new WaitForSeconds(0.5f);
             uavsManager.GenerateUavs(jsonSerializerTest.rootObject.UavsRecords);
             yield return new WaitForSeconds(0.5f);
             pathsManager.GeneratePaths(jsonSerializerTest.rootObject.UavPathsRecords);
             yield return new WaitForSeconds(0.5f);
             uavsManager.NavigateAll();
            }

            
        }
    }
}