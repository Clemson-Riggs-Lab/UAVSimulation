using System.Collections;
using System.Collections.Generic;
using System.IO;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using Menu;
using Newtonsoft.Json;
using UnityEngine;

public class JsonSerializerTest : MonoBehaviour
{		
  //  [SerializeField] public ConsoleTextHandler consoleTextHandler;
    public RootObject rootObject = new RootObject();
    
    public bool GetDefaultSettings { get; set; }= true;
    public bool AddDefaultWaypointsRecords { get; set; } = true;
    public bool AddDefaultUavRecords { get; set; } = true;
    public bool AddDefaultFuelLeaksRecord { get; set; } = true;
    public bool AddDefaultUavPathsRecords { get; set; } = true;
    private void OnValidate()
    {
       //  MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(consoleTextHandler, this, this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GetDefaultSettings == true) rootObject.Settings = DefaultRecordsCreator.GetDefaultSettings();
        if (AddDefaultWaypointsRecords == true) rootObject.WaypointsRecords = DefaultRecordsCreator.AddDefaultWaypointsRecords();
        if (AddDefaultUavRecords == true) rootObject.UavsRecords = DefaultRecordsCreator.AddDefaultUavRecords();
        if (AddDefaultFuelLeaksRecord == true) rootObject.FuelLeaksRecord = DefaultRecordsCreator.AddDefaultFuelLeaksRecord();
		if (AddDefaultUavPathsRecords == true) rootObject.UavPathsRecords = DefaultRecordsCreator.AddDefaultUavPathsRecords();
        string json = JsonConvert.SerializeObject(rootObject, Formatting.Indented);
        //  consoleTextHandler.AddTextToConsole(json);

        using StreamWriter file = File.CreateText(@"D:\mytest.json");
        JsonSerializer serializer = new JsonSerializer();
        file.Write(json);
    }
    
}
