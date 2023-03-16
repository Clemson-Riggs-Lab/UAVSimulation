using DG.Tweening;
using UnityEngine;

namespace Modules.TargetDetection
{
    public class Target:MonoBehaviour
    { 
        public int WayPointId{ get; set; }
        public ObjectType ObjectType { get; set; } = ObjectType.Box;
        public bool RealTarget { get; set; }=false;
        
        private Sequence _sequence;
        
        public void Initialize(int waypointId, ObjectType objectType, Vector3 position, int gameObjectLayer, bool realTarget)
        {
            WayPointId = waypointId;
            ObjectType=objectType;
            transform.position = position;
            var o = gameObject;
            o.name="Target "+waypointId;
            o.layer = gameObjectLayer;
            RealTarget = realTarget;
            if(realTarget)
                o.GetComponent<Renderer>().material.color = HelperScripts.ColorHelper.StringToColor( GameManager.Instance.settingsDatabase.targetDetectionSettings.targetColor);
            else
            {
                o.GetComponent<Renderer>().material.color = HelperScripts.ColorHelper.StringToColor( GameManager.Instance.settingsDatabase.targetDetectionSettings.nonTargetColor);
            }
            StartRotation();
        }

        private void StartRotation()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DORotate(new Vector3(180, 180, 180), 3f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
            _sequence.SetLoops(-1);
        }
        
       private  void OnDisable()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }
        }
    }
    
    
    public enum  ObjectType
    {
        Box,
        Uav
    }
}