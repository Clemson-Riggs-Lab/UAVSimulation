using System;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints;

namespace VisualObjects
{
    public class VisualObject:MonoBehaviour
    {
        [NonSerialized] private WayPoints.WayPoint _wayPoint;
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