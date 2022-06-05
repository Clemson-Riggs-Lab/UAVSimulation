using System;
using UnityEngine;

namespace UAVs
{
    public class FuelManager : MonoBehaviour
    {
        [NonSerialized]public Uav Uav;
        private bool _isLeaking;

        public bool IsLeaking
        {
            get => _isLeaking;
            set => _isLeaking = value;
        }

        private void Start()
        {
            gameObject.TryGetComponent(out Uav);
            if (Uav != null) return;
            // else there is no UAV script attached.
            Debug.Log( "Game object was destroyed because the attached fuel manager requires an attached UAV script",gameObject);
            Destroy(gameObject);// a fuel manager should only be attached to a game object that has a uav script
        }
    }
}