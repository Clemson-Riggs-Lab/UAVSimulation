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
    private UavSettingsSO uavSettings;
    private UavCameraAndTargetDetectionPanelSettingsSO _uavCameraAndTargetDetectionPanelSettings;
    
    private SettingsDatabaseSO _settingsDatabase;
    
    public bool GetDefaultSettings { get; set; }= true;
    public bool AddDefaultWayPointsRecords { get; set; } = true;
    public bool AddDefaultUavRecords { get; set; } = true;
    public bool AddDefaultFuelLeaksRecord { get; set; } = true;
    public bool AddDefaultUavPathsRecords { get; set; } = true;
    public bool AddDefaultPromptRecords { get; set; } = true;
    public bool AddDefaultNFZRecords { get; set; } = true;
   
    //Start is called before the first frame update
    void Start()
    {
        if (AddDefaultWayPointsRecords == true) inputRecordsDatabaseSO.WayPointsRecords = DefaultRecordsCreator.GetDefaultWayPointsRecords();
        if (AddDefaultUavRecords == true) inputRecordsDatabaseSO.UavsRecords = DefaultRecordsCreator.GetDefaultUavRecords();
        if (AddDefaultFuelLeaksRecord == true) inputRecordsDatabaseSO.FuelLeaksRecord = DefaultRecordsCreator.GetDefaultFuelLeaksRecord();
		if (AddDefaultUavPathsRecords == true) inputRecordsDatabaseSO.UavPathsRecords = DefaultRecordsCreator.GetDefaultUavPathsRecords();
		if(AddDefaultPromptRecords==true) inputRecordsDatabaseSO.Prompts = DefaultRecordsCreator.AddDefaultPromptRecords();
		if(AddDefaultNFZRecords==true) inputRecordsDatabaseSO.NFZRecords = DefaultRecordsCreator.GetDefaultNFZRecords();
		
		
		var settingsDatabase = GameManager.Instance.settingsDatabase;
		string json = JsonConvert.SerializeObject(inputRecordsDatabaseSO, Formatting.Indented);
        //  consoleTextHandler.AddTextToConsole(json);
  
        using StreamWriter file = File.CreateText(@"D:\mytest.json");
        JsonSerializer serializer = new JsonSerializer();
        file.Write(json);
        
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);
       // var serializer2 = new YamlDotNet.Serialization.Serializer();
        //string yaml = serializer2.Serialize(deserializedObject);
       // using StreamWriter file2 = File.CreateText(@"D:\mytest4.json");
       // file2.Write(yaml);
        //file2.Close();
     //    navigationSettings= GameManager.Instance.settingsDatabase.navigationSettings;
     //    uavGeneralSettings = GameManager.Instance.settingsDatabase.uavGeneralSettings;
     //    promptSettings = GameManager.Instance.settingsDatabase.promptSettings;
     //    nfzSettings = GameManager.Instance.settingsDatabase.nfzSettings;
     //    uavCameraAndTargetDetectionPanelSettings = GameManager.Instance.settingsDatabase.uavCameraAndTargetDetectionPanelSettings;
     //    uavSettings = GameManager.Instance.settingsDatabase.uavSettings;
     //    settingsDatabase = GameManager.Instance.settingsDatabase;
     //    
     //    string json1 = JsonConvert.SerializeObject(navigationSettings, Formatting.Indented);
     //    using StreamWriter file1 = File.CreateText(@"D:\mytest1.json");
     //    file1.Write(json1);
     //    file1.Close();
     //    
     //    string json2 = JsonConvert.SerializeObject(uavGeneralSettings, Formatting.Indented);
     //    using StreamWriter file3 = File.CreateText(@"D:\UavGeneralSettings.json");
     //    
     //    file3.Write(json2);
     //    file3.Close();
     //    
     //    string json3 = JsonConvert.SerializeObject(promptSettings, Formatting.Indented);
     //    using StreamWriter file4 = File.CreateText(@"D:\PromptSettings.json");
     //    
     //    file4.Write(json3);
     //    file4.Close();
     //    
     //    string json4 = JsonConvert.SerializeObject(nfzSettings, Formatting.Indented);
     //    using StreamWriter file5 = File.CreateText(@"D:\NFZSettings.json");
     //    
     //    file5.Write(json4);
     //    file5.Close();
     //    
     //    string json5 = JsonConvert.SerializeObject(uavCameraAndTargetDetectionPanelSettings, Formatting.Indented);
     //    using StreamWriter file6 = File.CreateText(@"D:\UavCameraAndTargetDetectionSettings.json");
     //    
     //    file6.Write(json5);
     //    file6.Close();
     //    
     //    string json6 = JsonConvert.SerializeObject(uavSettings, Formatting.Indented);
     //    using StreamWriter file7 = File.CreateText(@"D:\UavSettings.json");
     //    
     //    file7.Write(json6);
     //    file7.Close();
     //    
        string json7 = JsonConvert.SerializeObject(settingsDatabase, Formatting.Indented);
        using StreamWriter file8 = File.CreateText(@"D:\SettingsDatabase.json");
        
        file8.Write(json7);
	    file8.Close();   
     
      //  JsonSerializer serializer1= new JsonSerializer();
        // readfile to string
        // string json2 = File.ReadAllText(@"D:\mytest2.json");
        // JsonUtility.FromJsonOverwrite(json2, navigationSettings);
       // NavigationSettings navigationSettings1 = (NavigationSettings)serializer1.Deserialize(new JsonTextReader(new StringReader(json2)), typeof(NavigationSettings));
       // Debug.Log(navigationSettings1.fixedSpeed+"gaddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd");
    }
    
}
