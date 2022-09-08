using UnityEngine;

namespace TargetDetection
{
    public class Target:MonoBehaviour
    { 
        public int WayPointId{ get; set; }
        public ObjectType ObjectType { get; set; } = ObjectType.Box;
        
        public void Initialize(int waypointId, ObjectType objectType, Vector3 position, int gameObjectLayer)
        {
            WayPointId = waypointId;
            ObjectType=objectType;
            transform.position = position;
            var o = gameObject;
            o.name="Target "+waypointId;
            o.layer = gameObjectLayer;
        }
    }
    
    
    public enum  ObjectType
    {
        Box,
        Uav
    }
}