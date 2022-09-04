using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Events.ScriptableObjects;
using HelperScripts;
using Menu;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOHandlers
{
    /// <summary>
    /// This class provides functionalities for getting all the input files from the input files directory
    /// InputFiles directory is within the Application.dataPath directory. (i.e., Application.dataPath/InputFiles.)
    /// The class should be attached to a gameObject.
    /// It is highly recommended that all Input File handling be done in this class (i.e., setting file folders, setting file types, etc.)
    /// </summary>
    public class InputFilesHandler:MonoBehaviour
    {
        [SerializeField] private ConsoleMessageEventChannelSO writeMessageToConsoleChannel;
        [SerializeField] private StringEventChannelSO inputFileSelectedChannel;
        
        private const string InputFilesExtension = "json";
        
        [Header("Input Folders Names within the /InputFiles directory")]
        [SerializeField] public string baseInputFolderName =FilesType.SettingsAndConfigs.ToString(); //base input folder contains base input files which contain t
        
        private string _baseInputFolderPath;
        private FileInfo _selectedBaseInputFileInfo=null;
        

        public FileInfo SelectedBaseInputFileInfo
        {
            get => _selectedBaseInputFileInfo;
            set
            {
                Debug.Log(value.FullName);
                _selectedBaseInputFileInfo = value;
                if (inputFileSelectedChannel != null)
                {
                    inputFileSelectedChannel.RaiseEvent(value.FullName);
                }
            }
        }
        public enum FilesType
        {
            SettingsAndConfigs,
            Settings,
            Configs
        }
        
        public  void OnEnable()
        {
            var inputFolder = Application.dataPath + "/InputFiles/";
            _baseInputFolderPath = inputFolder + baseInputFolderName+"/";

            // Create Directory if it doesnt Exist
            if (!Directory.Exists(inputFolder)) Directory.CreateDirectory(inputFolder);
            if (!Directory.Exists(_baseInputFolderPath)) Directory.CreateDirectory(_baseInputFolderPath);
        }
        
        public List<FileInfo> GetFilesInfoFromWorkDir(FilesType filesType)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_baseInputFolderPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*."+filesType.ToString()+"." + InputFilesExtension);
            
            if (fileInfos.Length != 0)
            {
                List<FileInfo> orderedList = fileInfos.OrderBy(x => x.LastAccessTime).ToList(); 
                return orderedList;
            }
            else
            {
                RaiseError("No files of type "+ filesType.ToString()+"  found in: " + directoryInfo.FullName);
                return new List<FileInfo>();
            }
        }

        public string LoadFile(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            else
            {
                RaiseError("Selected File Does Not Exist");
                return string.Empty;
            }
        }

        private void RaiseError(string errorText)
        {
            if(writeMessageToConsoleChannel != null)
            writeMessageToConsoleChannel.RaiseEvent("",new() {text= errorText,color = "red"});
        }
    }
}