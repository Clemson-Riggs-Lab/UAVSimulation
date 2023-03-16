using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Databases.ScriptableObjects;
using HelperScripts;
using IOHandlers;
using Modules.Navigation.Settings.ScriptableObjects;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using Modules.Prompts.Settings.ScriptableObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UAVs.Settings.ScriptableObjects;
using UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects;
using UnityEngine;


public class JsonSerializerTest : MonoBehaviour
{		
    public InputRecordsDatabaseSO inputRecordsDatabaseSO;
    private NavigationSettingsSO _navigationSettings;
    private UavSettingsSO _uavSettings;
    private PromptSettingsSO _promptSettings;
    private NFZSettingsSO _nfzSettings;
    private UavCameraAndTargetDetectionPanelSettingsSO _uavCameraAndTargetDetectionPanelSettings;
    
    private SettingsDatabaseSO _settingsDatabase;
    
    public bool GetDefaultSettings { get; set; }= true;
    public bool AddDefaultWayPointsRecords { get; set; } = true;
    public bool AddDefaultUavRecords { get; set; } = true;
    public bool AddDefaultFuelLeaksRecord { get; set; } = true;
    public bool AddDefaultPromptRecords { get; set; } = true;
    public bool AddDefaultNFZRecords { get; set; } = true;
    
    public bool AddDefaultDynamicNavigationWorkloadRecords { get; set; } = true;
    public bool AddDefaultDynamicTargetDetectionWorkloadRecords { get; set; } = true;
   
    //Start is called before the first frame update
    void Start()
    {
        if (AddDefaultFuelLeaksRecord == true) inputRecordsDatabaseSO.FuelLeaksRecord = DefaultRecordsCreator.GetDefaultFuelLeaksRecord();
		if(AddDefaultPromptRecords==true) inputRecordsDatabaseSO.Prompts = DefaultRecordsCreator.AddDefaultPromptRecords();
		if(AddDefaultNFZRecords==true) inputRecordsDatabaseSO.NFZRecords = DefaultRecordsCreator.GetDefaultNFZRecords();
		if (AddDefaultDynamicNavigationWorkloadRecords == true) inputRecordsDatabaseSO.UavDynamicNavigationWorkloadRecords = DefaultRecordsCreator.GetDynamicNavigationWorkloadRecords();
		if (AddDefaultDynamicTargetDetectionWorkloadRecords == true) inputRecordsDatabaseSO.UavDynamicTargetDetectionWorkloadRecords = DefaultRecordsCreator.GetUavDynamicTargetDetectionWorkloadRecords();
		
		
		
		var settingsDatabase = GameManager.Instance.settingsDatabase;
		string json = JsonConvert.SerializeObject(inputRecordsDatabaseSO, Formatting.Indented);
  
        using StreamWriter file = File.CreateText(@"D:\mytest.json");
        JsonSerializer serializer = new JsonSerializer();
        file.Write(json);
        
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);

        string json7 = JsonConvert.SerializeObject(settingsDatabase, Formatting.Indented);
        using StreamWriter file8 = File.CreateText(@"D:\SettingsDatabase.json");
        
        file8.Write(json7);
	    file8.Close();
	    
    }
    
}
