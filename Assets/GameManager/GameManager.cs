using System;
using System.Collections;
using System.IO;
using Prompts;
using HelperScripts;
using IOHandlers;
using Newtonsoft.Json;
using NoFlyZone;
using ScriptableObjects.Databases;
using ScriptableObjects.InputFiles;
using ScriptableObjects.UAVs.Navigation;
using TargetDetection;
using UAVs;
using UAVs.Sub_Modules.FuelAndHealth;
using UAVs.Sub_Modules.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;
using static HelperScripts.Enums;

public class GameManager : MonoBehaviour
{
        public static GameManager Instance { get; private set; }
        
        [SerializeField] public GameObject wayPointsContainer;
        [SerializeField] public GameObject uavsContainer;
        [SerializeField] public GameObject terrainContainer;
        [SerializeField] public GameObject nfzsContainer;
        [Space(20)]
        
        [SerializeField] public ConfigFilesDatabaseSO configFilesDatabase;
        
        [Space(20)]
        [SerializeField] public WayPointsManager wayPointsManager;
        [SerializeField] public UavsManager uavsManager;
        [Space(20)]
     //   [SerializeField] public JsonSerializerTest jsonSerializerTest;
        [Space(20)]
        [SerializeField] public NavigationManager navigationManager;
        
        [Space(20)]
        [SerializeField] public FuelAndHealthManager fuelAndHealthManager;
        
        [Space(20)]
        [SerializeField] public PromptsManager promptsManager;
        [Space(20)]
        [SerializeField] public NFZsManager nfzsManager;
        [Space(20)]
        [SerializeField] public TargetsManager targetsManager;
        
        [Space(20)]
        [SerializeField] public InputRecordsDatabaseSO inputRecordsDatabase;
        [SerializeField] public ChannelsDatabaseSO channelsDatabase;
        [SerializeField] public PrefabsDatabaseSO prefabsDatabase;
        [SerializeField] public SettingsDatabaseSO settingsDatabase;
        
        private bool _forceGenerateFromRecords = true;
       [NonSerialized] public float simulationStartTime;

       [SerializeField] public BlockingPanelController blockingPanelController;

        private void OnValidate()
        {
            AssertionHelper.AssertComponentReferencedInEditor(wayPointsContainer,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(uavsContainer,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(terrainContainer,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(nfzsContainer,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(configFilesDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(wayPointsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(navigationManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(fuelAndHealthManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(promptsManager,this,this.gameObject);
            //AssertionHelper.AssertComponentReferencedInEditor(jsonSerializerTest,this,this.gameObject);
            
            AssertionHelper.AssertComponentReferencedInEditor(inputRecordsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(channelsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(prefabsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(settingsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(nfzsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(targetsManager,this,this.gameObject);
            
            AssertionHelper.AssertComponentReferencedInEditor(blockingPanelController,this,this.gameObject);
            
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
            if(configFilesDatabase.settingsFileFullFilePath != "")
            {try
                {
                    var settings = File.ReadAllText(configFilesDatabase.settingsFileFullFilePath);
                    JsonConvert.PopulateObject(settings, settingsDatabase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError("Error while reading settings file:, defaulted to default settings");
                    _forceGenerateFromRecords = true;
                }}

            if (configFilesDatabase.settingsFileFullFilePath != "")
            {
                try
                {
                    var inputRecords = File.ReadAllText(configFilesDatabase.inputFileFullFilePath);
                    JsonConvert.PopulateObject(inputRecords, inputRecordsDatabase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError("Error while reading input file:, defaulted to default input records");
                    _forceGenerateFromRecords = true;
                }

            }
            else _forceGenerateFromRecords = true;
            StartCoroutine(InitializeSimulation());

            MonitorSimulationEnd();

        }

      

        private IEnumerator InitializeSimulation()
        {
            wayPointsManager.Initialize();
            uavsManager.Initialize();
            navigationManager.Initialize();
            nfzsManager.Initialize();
            promptsManager.Initialize();
            fuelAndHealthManager.Initialize();
            
            if (!_forceGenerateFromRecords)
            {
                settingsDatabase.uavSettingsDatabase.navigationSettings.navigationRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.uavSettingsDatabase.uavGeneralSettings.uavRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.waypointSettings.waypointsRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.nfzSettings.nfzRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.uavSettingsDatabase.fuelAndHealthSettings.fuelLeaksRecordsSource = InputRecordsSource.FromDefaultRecords;
                
                
            }
            
            simulationStartTime = Time.time+ 5f;
            
            blockingPanelController.LoadingView(simulationStartTime);
            
            wayPointsManager.GenerateWayPoints(); 
            yield return new WaitForSeconds(0.1f);
            uavsManager.GenerateUavs(); 
            yield return new WaitForSeconds(0.1f);
            navigationManager.GeneratePaths(); 
            yield return new WaitForSeconds(0.1f);
            nfzsManager.LoadNFZs();
            yield return new WaitForSeconds(0.1f);
            promptsManager.LoadPrompts();
            yield return new WaitForSeconds(0.1f);
            
            
            StartCoroutine( navigationManager.NavigateAll(simulationStartTime));
            StartCoroutine( nfzsManager.StartNFZsTimerCoroutine(simulationStartTime));
            StartCoroutine( promptsManager.StartPromptsTimerCoroutine(simulationStartTime));
            StartCoroutine( fuelAndHealthManager.StartFuelAndHealthControllers(simulationStartTime));
            
        }
        
        private void MonitorSimulationEnd()
        { 
            channelsDatabase.uavChannels.fuelAndHealthChannels.uavLostEventChannel.Subscribe(OnUavLost);
        }

        private int _uavsLost = 0;
        private void OnUavLost(Uav arg0)
        {
            _uavsLost++;
           if(_uavsLost==uavsManager.uavs.Count)
               blockingPanelController.ClosingView();
        }
}
