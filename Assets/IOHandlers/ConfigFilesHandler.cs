using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelperScripts;
using IOHandlers.Settings.ScriptableObjects;
using Multiplayer;
using ScriptableObjects.EventChannels;
using UI.Console.Channels.ScriptableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static IOHandlers.ConfigFilesHandler;

namespace IOHandlers
{
    /// <summary>
    /// This class provides functionalities for getting all the config files from the config files directory
    /// configFiles directory is within the Application.dataPath directory. (i.e., Application.dataPath/configFiles.)
    /// The class should be attached to a gameObject.
    /// It is highly recommended that all config File handling be done in this class (i.e., setting file folders, setting file types, etc.)
    /// </summary>
    public class ConfigFilesHandler:MonoBehaviour
    {
        [SerializeField] private ConsoleMessageEventChannelSO writeMessageToConsoleChannel;
        [SerializeField] public ConfigFilesSettingsSO configFilesSettings;
        private const string ConfigFilesExtension = "json";
        
        [Header("Folders Names within the /InputFiles directory")]
        [SerializeField] public string inputFolderName =FilesType.Input.ToString(); // input folder contains input files
        [SerializeField] public string settingsFolderName =FilesType.Settings.ToString(); // settings folder contains settings files
        
        private string _inputFolderPath;
        private string _settingsFolderPath;


        private string _inputFileName = "sample";
        private string _settingsFileName = "sample";

        public enum FilesType
        {
            Input,
            Settings, 
        }
        
        public void OnEnable()
        {
            Debug.Log(Application.dataPath);
            var configsFolder = Application.dataPath + "/configFiles/";
            _inputFolderPath = configsFolder + inputFolderName + "/";
            _settingsFolderPath = configsFolder + settingsFolderName + "/";

            // Create Directory if it doesnt Exist
            if (!Directory.Exists(configsFolder)) Directory.CreateDirectory(configsFolder);
            if (!Directory.Exists(_inputFolderPath)) Directory.CreateDirectory(_inputFolderPath);
            if (!Directory.Exists(configsFolder + settingsFolderName+"/")) Directory.CreateDirectory(configsFolder + settingsFolderName + "/");
        }

        private void Start()
        {
            MainMenuNetworkCallsHandler.Instance.NewInputFileReceived_NetworkEventHandler += OnNewInputFileReceviedNetworkEventHandler;
            MainMenuNetworkCallsHandler.Instance.NewSettingsFileReceived_NetworkEventHandler += OnNewSettingsFileReceviedNetworkEventHandler;
        }

        public List<FileInfo> GetFilesInfoFromWorkDir(FilesType filesType)
        {
            DirectoryInfo directoryInfo = null;
            if(filesType == FilesType.Input)
                directoryInfo = new DirectoryInfo(_inputFolderPath);
            else
            {
                directoryInfo = new DirectoryInfo(_settingsFolderPath);
            }
            FileInfo[] fileInfos = directoryInfo.GetFiles("*."+filesType.ToString()+"." + ConfigFilesExtension);
            
            if (fileInfos.Length != 0)
            {
                List<FileInfo> orderedList = fileInfos.OrderBy(x => x.LastAccessTime).ToList(); 
                return orderedList;
            }
            else
            {
                WriteToConsole("No files of type "+ filesType.ToString()+"  found in: " + directoryInfo.FullName,"red");
                return new List<FileInfo>();
            }
        }


        public void WriteToConsole(string errorText,string color)
        {
            if(writeMessageToConsoleChannel != null)
                writeMessageToConsoleChannel.RaiseEvent("",new() {text= errorText,color = color});
        }

        public void SelectFile(FileInfo fileInfo, FilesType fileType)
        {
            switch (fileType)
            {
                //else select file in database
                case FilesType.Input:
                    configFilesSettings.inputFileFullFilePath = fileInfo.FullName;
                    break;
                case FilesType.Settings:
                    configFilesSettings.settingsFileFullFilePath = fileInfo.FullName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        private void OnNewInputFileReceviedNetworkEventHandler(object sender, string newJsonStr)
        {
            List<FileInfo> fileInfoLs = GetFilesInfoFromWorkDir(FilesType.Input);

            if (fileInfoLs == null || fileInfoLs.Count == 0)
            {
                File.WriteAllText(_inputFolderPath + _inputFileName + "." + FilesType.Input.ToString() + "." + ConfigFilesExtension, newJsonStr);
                List<FileInfo> newFileInfoLs = GetFilesInfoFromWorkDir(FilesType.Input);
                SelectFile(newFileInfoLs[0], FilesType.Input);
            }
            else
            {
                string oldJsonStr = File.ReadAllText(fileInfoLs[0].FullName);

                if (oldJsonStr.Equals(newJsonStr) == false)
                {
                    File.WriteAllText(fileInfoLs[0].FullName, newJsonStr);
                }
            }
        }        
        
        private void OnNewSettingsFileReceviedNetworkEventHandler(object sender, string newJsonStr)
        {
            List<FileInfo> fileInfoLs = GetFilesInfoFromWorkDir(FilesType.Settings);

            if (fileInfoLs == null || fileInfoLs.Count == 0)
            {
                File.WriteAllText(_settingsFolderPath + _settingsFileName + "." + FilesType.Settings.ToString() + "." + ConfigFilesExtension, newJsonStr);
                List<FileInfo> newFileInfoLs = GetFilesInfoFromWorkDir(FilesType.Settings);
                SelectFile(newFileInfoLs[0], FilesType.Settings);
            }
            else
            {
                string oldJsonStr = File.ReadAllText(fileInfoLs[0].FullName);

                if (oldJsonStr.Equals(newJsonStr) == false)
                {
                    File.WriteAllText(fileInfoLs[0].FullName, newJsonStr);
                }
            }
        }
    }
}