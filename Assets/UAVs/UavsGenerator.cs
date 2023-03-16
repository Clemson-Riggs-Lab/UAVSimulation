using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers;
using UAVs.Settings.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;
using Random = System.Random;

namespace UAVs
{
    public class UavsGenerator : MonoBehaviour
    {
        private GameObject _uavPrefab;
        private GameObject _uavsContainer;
        private GameObject _wayPointsContainer;
        private WayPointsManager _wayPointsManager;
        private UavsManager _uavsManager;
        private UavSettingsSO _uavSettings;
        private int _uavIterator;
        private Random _waypointsShuffleRandomGenerator = new Random();

        public void Initialize()
        {
            GetReferencesFromGameManager();
            _uavsContainer.transform.position = _wayPointsContainer.transform.position;
            _uavIterator = 0;
            _waypointsShuffleRandomGenerator = new Random( _uavSettings.waypointsShuffleRandomGeneratorSeed);
        }

        private void GetReferencesFromGameManager()
        {
            _wayPointsManager = GameManager.Instance.wayPointsManager;
            _uavsManager = GameManager.Instance.uavsManager;
            _uavsContainer = GameManager.Instance.uavsContainer;
            _wayPointsContainer = GameManager.Instance.wayPointsContainer;
            _uavPrefab = GameManager.Instance.prefabsDatabase.uavPrefab;
            _uavSettings = GameManager.Instance.settingsDatabase.uavSettings;
        }

        public List<Uav> GenerateUavsRandomly(int numberOfUavsToGenerate=0)
        {
            var uavs = new List<Uav>();
            if (numberOfUavsToGenerate == 0)
            {
                numberOfUavsToGenerate = _uavSettings.numberOfUaVsToMaintain;
            }
            
            var wayPoints = _wayPointsManager.wayPoints;
            wayPoints.Shuffle(_waypointsShuffleRandomGenerator);
            var uavCount = _uavIterator;

            var nfzLayer = LayerMask.NameToLayer("NFZ");

            for (int i = 0; i < wayPoints.Count && uavCount < _uavIterator + numberOfUavsToGenerate; i++)
            {
                if (!Physics.CheckSphere(wayPoints[i].transform.position, 5, 1 << nfzLayer))
                {
                    uavs.Add( GenerateUav(uavCount++, wayPoints[i]));
                }
            }
            _uavIterator = uavCount;
            
            return uavs;
        }

        private Uav GenerateUav(int id, WayPoint wayPoint)
        {
            var uav = Instantiate(_uavPrefab, wayPoint.transform.position, Quaternion.Euler(0, 90, 0), _uavsContainer.transform).GetComponent<Uav>();
            uav.Initialize(id, wayPoint);
            
            return uav;
        }
    }
}
