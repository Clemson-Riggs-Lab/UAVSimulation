using System;
using System.Collections;
using System.IO;
using Databases.ScriptableObjects;
using HelperScripts;
using IOHandlers;
using IOHandlers.Settings.ScriptableObjects;
using Modules.FuelAndHealth;
using Modules.Navigation;
using Modules.Navigation.Submodules.Rerouting;
using Modules.NoFlyZone;
using Modules.Prompts;
using Modules.TargetDetection;
using Newtonsoft.Json;
using UAVs;
using UI;
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
        
        [SerializeField] public ConfigFilesSettingsSO configFilesSettings;
        
        [Space(20)]
        [SerializeField] public WayPointsManager wayPointsManager;
        [SerializeField] public UavsManager uavsManager;
        [SerializeField] public NavigationManager navigationManager;
        [SerializeField] public ReroutingManager reroutingManager;
        [SerializeField] public FuelManager fuelManager;
        [SerializeField] public PromptsManager promptsManager;
        [SerializeField] public NFZsManager nfzsManager;
        [SerializeField] public TargetsDetectionManager targetsDetectionManager;
        
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
            AssertionHelper.AssertComponentReferencedInEditor(configFilesSettings,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(wayPointsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(uavsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(navigationManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(reroutingManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(fuelManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(promptsManager,this,this.gameObject);
            
            AssertionHelper.AssertComponentReferencedInEditor(inputRecordsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(channelsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(prefabsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(settingsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(nfzsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(targetsDetectionManager,this,this.gameObject);
            
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
            SubscribeToChannels();
            if(configFilesSettings.settingsFileFullFilePath != "")
            {try
                {
                    var settings = File.ReadAllText(configFilesSettings.settingsFileFullFilePath);
                    JsonConvert.PopulateObject(settings, settingsDatabase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError("Error while reading settings file:, defaulted to default settings");
                    _forceGenerateFromRecords = true;
                }}

            if (configFilesSettings.settingsFileFullFilePath != "")
            {
                try
                {
                    var inputRecords = File.ReadAllText(configFilesSettings.inputFileFullFilePath);
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

        }

        private void SubscribeToChannels()
        {
            channelsDatabase.simulationEndedEventChannel.Subscribe(OnSimulationEndEvent);
        }

      


        private IEnumerator InitializeSimulation()
        {
            wayPointsManager.Initialize();
            uavsManager.Initialize();
            navigationManager.Initialize();
            nfzsManager.Initialize();
            promptsManager.Initialize();
            fuelManager.Initialize();
            
            if (_forceGenerateFromRecords)
            {
                settingsDatabase.navigationSettings.navigationRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.uavSettings.uavRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.waypointSettings.waypointsRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.nfzSettings.nfzRecordsSource = InputRecordsSource.FromDefaultRecords;
                settingsDatabase.fuelSettings.fuelLeaksRecordsSource = InputRecordsSource.FromDefaultRecords;
                
                
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
            StartCoroutine( fuelManager.StartFuelControllers(simulationStartTime));
            
        }
        
        
        
        private void OnSimulationEndEvent()
        {
            blockingPanelController.ClosingView();
        }
}
