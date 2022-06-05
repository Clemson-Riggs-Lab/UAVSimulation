using System;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace VisualObjects
{
    public class VisualObject:MonoBehaviour
    {
        [NonSerialized] private Waypoints.Waypoint _waypoint;
        [NonSerialized] private Transform _transform;
        [NonSerialized] public string Type;
        void  Start()
        {
            _transform = transform;
            Type=this.GetType().Name;

        }
    }

    public class Target : VisualObject
    {
        [NonSerialized]public TargetType TargetType = TargetType.Soldier;
    }

    public class NonTarget : VisualObject
    {
   

    }

    public enum  TargetType
    {
        Soldier,
        Tank,
        Airplane,
        Uav
    }
}