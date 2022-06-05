using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOHandlers
{
    public class InputFilesHandler
    {
        private static readonly string INPUT_FOLDER = Application.dataPath + "/InputFiles/";
        private static readonly string WAYPOINTS_INPUT_FOLDER =INPUT_FOLDER+ "Waypoints/";
        private static readonly string UAVs_INPUT_FOLDER =INPUT_FOLDER+ "UAVs/";
        
        private const string INPUT_FILES_EXTENSION = "json";
        
        public static void Init() 
        {
            // Create Directory if it doesnt Exist
            if (!Directory.Exists(INPUT_FOLDER)) Directory.CreateDirectory(INPUT_FOLDER);
            if (!Directory.Exists(WAYPOINTS_INPUT_FOLDER)) Directory.CreateDirectory(WAYPOINTS_INPUT_FOLDER);
            if (!Directory.Exists(UAVs_INPUT_FOLDER)) Directory.CreateDirectory(UAVs_INPUT_FOLDER);
        }

        public static List<FileInfo> GetListOfWaypointsInputFilesInDirectory()
        {
            return GetSortedListOfFilesInDirectory(WAYPOINTS_INPUT_FOLDER, "*." + INPUT_FILES_EXTENSION);
        }

        
        private static List<FileInfo> GetSortedListOfFilesInDirectory(string directory, string searchPattern)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            FileInfo[] files = directoryInfo.GetFiles(searchPattern);
            List<FileInfo> orderedList = files.OrderBy(x => x.LastAccessTime).ToList();
            return orderedList;
        }

        public static string LoadFile(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            else
            {
                RaiseError("Selected File Does Not Exist");
                return String.Empty;
            }
        }

        private static void RaiseError(string selectedFileDoesNotExist)
        {
            throw new System.NotImplementedException();
        }
    }
}