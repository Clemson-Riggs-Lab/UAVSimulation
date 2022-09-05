using UnityEngine;

namespace TargetDetection
{
    public class Target:MonoBehaviour
    { 
        public int WayPointId{ get; set; }
        public ObjectType ObjectType { get; set; } = ObjectType.Box;
        
        public void Initialize(int waypointId, ObjectType objectType, Vector3 position)
        {
            WayPointId = waypointId;
            ObjectType=objectType;
            transform.position = position;
            gameObject.name="Target "+waypointId;
        }
    }
    
    
    public enum  ObjectType
    {
        Box,
        Uav
    }
}