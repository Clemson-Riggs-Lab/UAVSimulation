using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelperScripts;
using IOHandlers.Settings.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UI.Console.Channels.ScriptableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
      
        

        public enum FilesType
        {
            Input,
            Settings,
            
        }
        
        public void OnEnable()
        {
            var configsFolder = Application.dataPath + "/configFiles/"; 
            _inputFolderPath = configsFolder + inputFolderName+"/";
            _settingsFolderPath = configsFolder + settingsFolderName+ "/";

            // Create Directory if it doesnt Exist
            if (!Directory.Exists(configsFolder)) Directory.CreateDirectory(configsFolder);
            if (!Directory.Exists(_inputFolderPath)) Directory.CreateDirectory(_inputFolderPath);
            if (!Directory.Exists(configsFolder + settingsFolderName+"/")) Directory.CreateDirectory(configsFolder + settingsFolderName+"/");
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
    }
}