using UnityEngine;

namespace Hexagon.ECS.Camera
{
    public class CameraImplementor : MonoBehaviour, ITransformComponent, ICameraComponent
    {
        private Transform cameraTransform;

        public void LookAt(Vector3 pos)
        {
            transform.LookAt(pos);
        }

        public Vector3 position
        {
            get { return cameraTransform.position; }
            set { cameraTransform.position = value; }
        }

        public Vector3 rotation
        {
            get { return cameraTransform.eulerAngles; }
            set { cameraTransform.eulerAngles = value; }
        }

        public Quaternion localRotation
        {
            get { return cameraTransform.localRotation; }
            set { cameraTransform.localRotation = value; }
        }

        private void Awake()
        {
            cameraTransform = transform;
        }
    }
}