using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helper_Scripts;
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
    /// The class requires a console to provide feedback to the user ( e.g., if a file is not found, if the directory is empty,etc.)
    /// The class should be attached to a gameObject.
    /// The class by default has the functionality for getting a base input file which should contain all the basic input and settings (i.e., waypoints, uavs, navigations, etc.)
    ///     Additional input settings should be ideally appended to the base input file, however, if you decide to go with multiple input files (e.g., tactor feedback is provided in a separate input file)
    ///     Then you should update the FilesType enum to include the new type of input files, and also implement the corresponding methods in the GetFilesInfo method.
    /// It is highly recommended that all Input File handling be done in this class (i.e., setting file folders, setting file types, etc.)
    /// </summary>
    public class InputFilesHandler:MonoBehaviour
    {
        [SerializeField]public ConsoleTextHandler consoleTextHandler;
        [SerializeField] public SettingsReviewManager settingsReviewManager;

        private static string _inputFolder;
        private const string InputFilesExtension = "json";
        
        [Header("Input Folders Names within the /InputFiles directory")]
        [SerializeField] public string baseInputFolderName ="baseInput"; //base input folder contains base input files which contain t
        private string _baseInputFolderPath;
        private FileInfo _selectedBaseInputFileInfo=null;
        

        public FileInfo SelectedBaseInputFileInfo
        {
            get => _selectedBaseInputFileInfo;
            set
            {
                _selectedBaseInputFileInfo = value;
                settingsReviewManager.SetKeyValuePair("Input File:", value.Name,
                    valueMessageType: MessageType.Info);
                Debug.Log(value.FullName);
            }
        }
        public enum FilesType
        {
            Base,
            Other
        }
        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(consoleTextHandler,this,this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(settingsReviewManager,this,this.gameObject);
        }

        public  void OnEnable()
        {
            _inputFolder = Application.dataPath + "/InputFiles/";
            _baseInputFolderPath = _inputFolder + baseInputFolderName+"/";

            // Create Directory if it doesnt Exist
            if (!Directory.Exists(_inputFolder)) Directory.CreateDirectory(_inputFolder);
            if (!Directory.Exists(_baseInputFolderPath)) Directory.CreateDirectory(_baseInputFolderPath);
        }
        
        public List<FileInfo> GetFilesInfo(FilesType filesType)
        {
            TRes Call<TRes>(Func<TRes> f) => f();

            return filesType switch
            {
                FilesType.Base => GetListOfBaseInputFilesInDirectory(),
                FilesType.Other => Call(()=> {RaiseError("File  type not implemented"); return new List<FileInfo>(); }),
                _  => Call(()=> {RaiseError("File type not supported"); return new List<FileInfo>(); })
            };
        }

        private List<FileInfo> GetListOfBaseInputFilesInDirectory()
        {
            return GetSortedListOfFilesInfoInDirectory(_baseInputFolderPath, "*." + InputFilesExtension);
        }


        private List<FileInfo> GetSortedListOfFilesInfoInDirectory(string directory, string searchPattern)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            FileInfo[] fileInfos = directoryInfo.GetFiles(searchPattern);
            
            if (fileInfos.Length != 0)
            {
                List<FileInfo> orderedList = fileInfos.OrderBy(x => x.LastAccessTime).ToList(); 
                return orderedList;
            }
            else
            {
                RaiseError("No files found in: " + directoryInfo.FullName, MessageType.Warning);
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

        private void RaiseError(string errorText, MessageType messageType =MessageType.Error)
        {
            consoleTextHandler.AddTextToConsole(errorText,messageType, doAnimate:true);
        }

      
    }
}