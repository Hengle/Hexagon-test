using Hexagon.ECS.Camera;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public class PlayerInputImplementor : MonoBehaviour, IInputComponent, ICameraTargetComponent, IImplementor
    {
        private Transform playerTransform;

        public Vector3 position
        {
            get { return playerTransform.position; }
        }

        public Vector3 rotation
        {
            get { return playerTransform.localEulerAngles; }
            set { playerTransform.localEulerAngles = value; }
        }

        public DispatchOnSet<HexCoordinates> selection { get; set; }
        public DispatchOnSet<HexCoordinates> target { get; set; }
        public DispatchOnSet<bool> pass { get; set; }
        public DispatchOnSet<int> abilitySelected { get; private set; }
        public DispatchOnSet<bool> cancel { get; private set; }
        public Ray camRay { get; set; }

        private void Awake()
        {
            playerTransform = transform;
            selection = new DispatchOnChange<HexCoordinates>(gameObject.GetInstanceID());
            target = new DispatchOnSet<HexCoordinates>(gameObject.GetInstanceID());
            pass = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            abilitySelected = new DispatchOnSet<int>(gameObject.GetInstanceID());
            cancel = new DispatchOnSet<bool>(gameObject.GetInstanceID());
        }
    }
}