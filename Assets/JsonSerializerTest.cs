using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using Menu;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ScriptableObjects.UAVs.Navigation;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class JsonSerializerTest : MonoBehaviour
{		
  //  [SerializeField] public ConsoleTextHandler consoleTextHandler;
    public RootObject rootObject = new RootObject();
    public NavigationSettingsSO navigationSettings;
    public bool GetDefaultSettings { get; set; }= true;
    public bool AddDefaultWayPointsRecords { get; set; } = true;
    public bool AddDefaultUavRecords { get; set; } = true;
    public bool AddDefaultFuelLeaksRecord { get; set; } = true;
    public bool AddDefaultUavPathsRecords { get; set; } = true;
    public bool AddDefaultPromptRecords { get; set; } = true;
    public bool AddDefaultNFZRecords { get; set; } = true;
   
    // Start is called before the first frame update
    void Start()
    {
        if (GetDefaultSettings == true) rootObject.Settings = DefaultRecordsCreator.GetDefaultSettings();
        if (AddDefaultWayPointsRecords == true) rootObject.WayPointsRecords = DefaultRecordsCreator.AddDefaultWayPointsRecords();
        if (AddDefaultUavRecords == true) rootObject.UavsRecords = DefaultRecordsCreator.AddDefaultUavRecords();
        if (AddDefaultFuelLeaksRecord == true) rootObject.FuelLeaksRecord = DefaultRecordsCreator.AddDefaultFuelLeaksRecord();
		if (AddDefaultUavPathsRecords == true) rootObject.UavPathsRecords = DefaultRecordsCreator.AddDefaultUavPathsRecords();
		if(AddDefaultPromptRecords==true) rootObject.Prompts = DefaultRecordsCreator.AddDefaultPromptRecords();
		if(AddDefaultNFZRecords==true) rootObject.NFZRecords = DefaultRecordsCreator.AddDefaultNFZRecords();
		
		
		
		string json = JsonConvert.SerializeObject(rootObject, Formatting.Indented);
        //  consoleTextHandler.AddTextToConsole(json);

        using StreamWriter file = File.CreateText(@"D:\mytest.json");
        JsonSerializer serializer = new JsonSerializer();
        file.Write(json);
        
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);
        var serializer2 = new YamlDotNet.Serialization.Serializer();
        string yaml = serializer2.Serialize(deserializedObject);
        using StreamWriter file2 = File.CreateText(@"D:\mytest4.json");
        file2.Write(yaml);
        file2.Close();
        navigationSettings= GameManager.Instance.settingsDatabase.uavSettings.navigationSettings;
        string json1 = JsonConvert.SerializeObject(navigationSettings, Formatting.Indented);
        using StreamWriter file1 = File.CreateText(@"D:\mytest1.json");
        JsonSerializer serializer1= new JsonSerializer();
        file1.Write(json1);
        file1.Close();
        // readfile to string
        // string json2 = File.ReadAllText(@"D:\mytest2.json");
        // JsonUtility.FromJsonOverwrite(json2, navigationSettings);
       // NavigationSettings navigationSettings1 = (NavigationSettings)serializer1.Deserialize(new JsonTextReader(new StringReader(json2)), typeof(NavigationSettings));
       // Debug.Log(navigationSettings1.fixedSpeed+"gaddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd");
    }
    
}
