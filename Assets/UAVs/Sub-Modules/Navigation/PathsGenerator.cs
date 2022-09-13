using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers.Records;
using ScriptableObjects.UAVs.Navigation;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using WayPoints;

namespace UAVs.Navigation
{
    public class PathsGenerator : MonoBehaviour
    {

        [SerializeField]public UavsManager uavsManager;
        [SerializeField]public WayPointsManager wayPointsManager;
        private NavigationSettingsSO navigationSettings;


        private void Start()
        {
            uavsManager= GameManager.Instance.uavsManager;
            wayPointsManager= GameManager.Instance.wayPointsManager;
            navigationSettings= GameManager.Instance.settingsDatabase.uavSettingsDatabase.navigationSettings;
        }
        
       public Dictionary<Uav,Navigator> GeneratePaths( List<UavPathsRecord> uavPathsRecords)
       {
           var navigators = new Dictionary<Uav,Navigator>();
              foreach (var uavPathsRecord in uavPathsRecords)
              { 
                var uav = uavsManager.uavs.FirstOrDefault(x => x.id == uavPathsRecord.UavId);
                
                if (uav == null)
                {
                    Debug.LogError("UAV not found while initializing paths from uavPathsRecords");
                    continue; //returning because the uav was not found (this should not happen)
                }
                
                if(!uav.gameObject.TryGetComponent(out Navigator navigator))
                    navigator = uav.gameObject.AddComponent<Navigator>();
                navigators[uav]=navigator;
                
                var paths = new List<Path>();
                var pathID = 0;
                foreach (var record in uavPathsRecord.PathRecords)
                {
                    if( wayPointsManager.TryGetWayPoint(record.DestinationWayPointID, out WayPoints.WayPoint destinationWayPoint)) //else we can't create the path since the destination waypoint could not be found
                    {
                        var path = new Path(pathID, uav, destinationWayPoint, record.UavVisuallyEnabled ?? false, record.TargetIsPresent ?? false);
                        
                        if (paths.Count > 0) // if the paths list is not empty, set the next path of the last item to this path, and the previous path of this path to the last item
                        {
                            paths.Last().nextPath = path;
                            path.previousPath = paths.Last();
                        }
                        paths.Add(path);
                        pathID++;
                    }
                }
                
                if(navigationSettings.loopingType!= NavigationSettingsSO.LoopType.Once)
                    paths=AddLoopingPaths(paths);
                
                navigator.Paths.AddRange(paths);
                
                
              }
          
           return navigators;
       }

       private List<Path> AddLoopingPaths(List<Path> paths)
       {
           if(paths.Count==0) // null check
               return paths;
           
           switch (navigationSettings.loopingType) 
           {
               case NavigationSettingsSO.LoopType.Once:
               default:
                   return paths;
               
               case NavigationSettingsSO.LoopType.Cycled: // we just add a reference to the first path in the last path
                   paths.Last().nextPath = paths.First();
                   paths.First().previousPath =  paths.Last();
                   return paths;
               
               case NavigationSettingsSO.LoopType.SeveralTimes:
                   var allPaths = new List<Path>(paths);
                   for (var i = 0; i < navigationSettings.numberOfLoops; i++)
                   {
                       var newPaths= new List<Path>(paths); //creating copy of the original paths list
                       allPaths.Last().nextPath = newPaths.First(); 
                       newPaths.First().previousPath = allPaths.Last();
                       allPaths.AddRange(paths);
                   }
                   return allPaths;
           }
       }
    }
}