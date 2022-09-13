using UnityEngine;

namespace MiniMap
{
    public class MiniMapCameraPositioning : MonoBehaviour
    {
        [SerializeField] private Transform terrainContainer;
        private void OnValidate()
        {
            PositionCamera();
        }
    
        void PositionCamera()
        {
        
            Camera camera = this.GetComponent<Camera>();

            Bounds objectBounds = terrainContainer.GetComponentInChildren<Terrain>().terrainData.bounds;
            camera.transform.position =new Vector3(objectBounds.center.x, 500, objectBounds.center.z);
            camera.orthographicSize= objectBounds.size.z / 2;
        
            // Vector3 triangleFarSideUpAxis = Quaternion.AngleAxis(0, terrainContainer.transform.right) * transform.forward;
            // //Calculate the up point of the triangle.
            // const float MARGIN_MULTIPLIER = 1f;
            // Vector3 triangleUpPoint = objectFrontCenter + triangleFarSideUpAxis * objectBounds.extents.y * MARGIN_MULTIPLIER;
            //
            // //The angle between the camera and the top point of the triangle is half the field of view.
            // //The tangent of this angle equals the length of the opposing triangle side over the desired distance between the camera and the object's front.
            // float desiredDistance = Vector3.Distance(triangleUpPoint, objectFrontCenter) / Mathf.Tan(Mathf.Deg2Rad * camera.GetComponent<Camera>().cameraFieldOfView / 2);
            //
            // camera.transform.position = -camera.transform.forward * desiredDistance + objectFrontCenter;
        }
    }
}
