using System.Collections;
using System.IO;
using Prompts;
using HelperScripts;
using Newtonsoft.Json;
using UAVs;
using UAVs.Navigation;
using UAVs.Navigation.ScriptableObjects;
using UAVs.Sub_Modules.Fuel;
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
        [SerializeField] public PromptsManager promptsManager;
        
        [Space(20)]
        [SerializeField] public ChannelsDatabase channelsDatabase;
        [SerializeField] public PrefabsDatabase prefabsDatabase;
        [SerializeField] public SettingsDatabase settingsDatabase;
        
        private bool _generateFromRecords = true;
        private void OnValidate()
        {
            AssertionHelper.AssertComponentReferencedInEditor(wayPointsContainer,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(uavsContainer,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(wayPointsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(navigationManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(fuelAndHealthManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(promptsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(jsonSerializerTest,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(channelsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(prefabsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(settingsDatabase,this,this.gameObject);
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
                wayPointsManager.Initialize();
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
                promptsManager.Initialize();
            }

            
        }
    }
