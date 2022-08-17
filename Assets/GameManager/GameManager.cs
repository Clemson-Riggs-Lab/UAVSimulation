using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Chat;
using Chat.ScriptableObjects;
using HelperScripts;
using Newtonsoft.Json;
using UAVs;
using UAVs.Fuel;
using UAVs.Fuel.ScriptableObjects;
using UAVs.Navigation;
using UAVs.Navigation.ScriptableObjects;
using UnityEngine;
using WayPoints;

public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] public GameObject wayPointsContainer;
        [SerializeField] public GameObject uavsContainer;
        [SerializeField] public GameObject terrainContainer;
        [Space(20)]
        [SerializeField] public WayPointsManager wayPointsManager;
        [SerializeField] public UavsManager uavsManager;
        [Space(20)]
        [SerializeField] public JsonSerializerTest jsonSerializerTest;
        [Space(20)]
        [SerializeField] public NavigationManager navigationManager;
        [SerializeField] public NavigationSettingsSO navigationSettings;
        
        [Space(20)]
        [SerializeField] public FuelAndHealthManager fuelAndHealthManager;
        
        [Space(20)]
        [SerializeField] public ChatManager chatManager;
        
        private bool _generateFromRecords = true;
        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(wayPointsContainer,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(uavsContainer,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(wayPointsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(navigationManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(fuelAndHealthManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(chatManager,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(jsonSerializerTest,this,this.gameObject);
        }

        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
    
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
        
        private void Start()
        {
            var json = File.ReadAllText(@"D:\mytest2.json");
            JsonConvert.PopulateObject(json, navigationSettings);
            

            wayPointsManager = wayPointsContainer.GetComponent<WayPointsManager>();
            uavsManager = uavsContainer.GetComponent<UavsManager>();
            StartCoroutine(InitializeSimulation());
            
        }

        private IEnumerator InitializeSimulation()
        {
            
            if (!_generateFromRecords)
            {
                wayPointsManager.GenerateWayPoints();
               uavsManager.GenerateUavs();
               navigationManager.GeneratePaths();
               navigationManager.NavigateAll();
            }
            else
            {
                
             wayPointsManager.GenerateWayPoints(jsonSerializerTest.rootObject.WayPointsRecords);
             yield return new WaitForSeconds(0.5f);
             uavsManager.GenerateUavs(jsonSerializerTest.rootObject.UavsRecords);
             yield return new WaitForSeconds(0.5f);
             navigationManager.GeneratePaths();
             yield return new WaitForSeconds(0.5f);
             navigationManager.NavigateAll();
             yield return new WaitForSeconds(0.5f);
             fuelAndHealthManager.Initialize();
             yield return new WaitForSeconds(0.5f);
             chatManager.Initialize();
            }

            
        }
    }
