using UnityEngine;

namespace TargetDetection
{
    public class VisualObject:MonoBehaviour
    { 
        public int WayPointId{ get; set; }
        public bool IsTarget { get; set; }
        public ObjectType ObjectType { get; set; } = ObjectType.TargetBox;
        
        public void Initialize(int waypointId,bool isTarget, ObjectType objectType, Vector3 position)
        {
            WayPointId = waypointId;
            IsTarget = isTarget;
            ObjectType=objectType;
            transform.position = position;
            gameObject.name="VisualObject "+waypointId;
        }
    }
    
    
    public enum  ObjectType
    {
        TargetBox,
        NonTargetBox,
        TargetSoldier,
        NonTargetSoldier,
        Tank,
        Airplane,
        Uav
    }
}