using System;
using System.Collections.Generic;
using UnityEngine;
using VisualObjects;

namespace Waypoints
{
    public class Waypoint : MonoBehaviour
    {
        [NonSerialized] public int ID;
        [NonSerialized] public Transform Transform ;
        [NonSerialized] public List<VisualObject> VisualObjects = new List<VisualObject>();
        [NonSerialized] public bool HasTargets = false;

        public Waypoint(int id=999, bool hasTargets=false)
        {
            this.ID = id;
            this.HasTargets = hasTargets;
        }

        // Start is called before the first frame update
        void Start()
        {
            Transform = gameObject.transform;
        }
    
    }
}


