using System;
using System.Collections;
using System.IO;
using Databases.ScriptableObjects;
using HelperScripts;
using IOHandlers.Settings.ScriptableObjects;
using Modules.FuelAndHealth;
using Modules.Navigation;
using Modules.Navigation.Submodules.Rerouting;
using Modules.NoFlyZone;
using Modules.Prompts;
using Modules.ScoreKeeper;
using Modules.ScoreKeeper.Settings;
using Modules.ScoreKeeper.Settings.ScriptableObjects;
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
        [SerializeField] public NFZsManager nfzsManager;
        [SerializeField] public TargetsDetectionManager targetsDetectionManager;
        [SerializeField] public ScoreManager scoreManager;

        [Space(20)]
        [SerializeField] public InputRecordsDatabaseSO inputRecordsDatabase;
        [SerializeField] public ChannelsDatabaseSO channelsDatabase;
        [SerializeField] public PrefabsDatabaseSO prefabsDatabase;
        [SerializeField] public SettingsDatabaseSO settingsDatabase;
        
       [SerializeField] public BlockingPanelController blockingPanelController;

       [NonSerialized] private bool _forceGenerateFromRecords = false;
       [NonSerialized] public float simulationStartTime;
       
       [NonSerialized] public int participantNumber = 0;
       [NonSerialized] public string trialNumber = "";      
       
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
            AssertionHelper.AssertComponentReferencedInEditor(nfzsManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(targetsDetectionManager,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(scoreManager,this,this.gameObject);
            
            AssertionHelper.AssertComponentReferencedInEditor(inputRecordsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(channelsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(prefabsDatabase,this,this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(settingsDatabase,this,this.gameObject);
            
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
            participantNumber= configFilesSettings.participantNumber;
            trialNumber = configFilesSettings.trialNumber;
            if(configFilesSettings.settingsFileFullFilePath != "")
            {
                try
                {
                    var settings = File.ReadAllText(configFilesSettings.settingsFileFullFilePath);
                    JsonConvert.PopulateObject(settings, settingsDatabase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError("Error while reading settings file:, defaulted to default settings");
                    _forceGenerateFromRecords = true;
                }
                
            }

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
            else
            {
                _forceGenerateFromRecords = true;
                Debug.LogError("Error while reading input file:, defaulted to default input records");

            }
            StartCoroutine(InitializeSimulation());

        }
        
        

        private IEnumerator InitializeSimulation()
        {
            
            if (_forceGenerateFromRecords)
            {
                settingsDatabase.nfzSettings.nfzRecordsSource = InputRecordsSource.FromDefaultRecords;
            }
            
            simulationStartTime = Time.time+ settingsDatabase.simulationStartDelay;
            blockingPanelController.LoadingView(simulationStartTime);
            
            nfzsManager.Initialize();
            StartCoroutine( nfzsManager.StartNFZsTimerCoroutine(simulationStartTime));
            yield return new WaitForSeconds(0.1f);
            
            wayPointsManager.Initialize();
            yield return new WaitForSeconds(0.1f);
            
            uavsManager.Initialize();
            scoreManager.Initialize();
            
            yield return new WaitForSeconds(0.1f);
            uavsManager.StartNavigation(simulationStartTime);
            
            yield return new WaitForSeconds(settingsDatabase.simulationDuration+ simulationStartTime - Time.time);
            channelsDatabase.simulationEndedEventChannel.RaiseEvent();
            Time.timeScale = 0f;   
            blockingPanelController.ClosingView();

        }
}
