using UnityEngine;

namespace Hexagon.Services.Map
{
    public class HexMapCamera : MonoBehaviour
    {
        private static HexMapCamera instance;

        public HexGrid grid;

        public float moveSpeedMinZoom, moveSpeedMaxZoom;
        private float rotationAngle;

        public float rotationSpeed;

        public float stickMinZoom, stickMaxZoom;
        private Transform swivel, stick;

        public float swivelMinZoom, swivelMaxZoom;
        private float zoom = 1f;

        public static bool Locked
        {
            set { instance.enabled = !value; }
        }

        public static void ValidatePosition()
        {
            instance.AdjustPosition(0f, 0f);
        }

        private void Awake()
        {
            swivel = transform.GetChild(0);
            stick = swivel.GetChild(0);
        }

        private void OnEnable()
        {
            instance = this;
        }

        private void Update()
        {
            var zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f) AdjustZoom(zoomDelta);

            var rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f) AdjustRotation(rotationDelta);

            var xDelta = Input.GetAxis("Horizontal");
            var zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f) AdjustPosition(xDelta, zDelta);
        }

        private void AdjustZoom(float delta)
        {
            zoom = Mathf.Clamp01(zoom + delta);

            var distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
            stick.localPosition = new Vector3(0f, 0f, distance);

            var angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
            swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }

        private void AdjustRotation(float delta)
        {
            rotationAngle += delta * rotationSpeed * Time.deltaTime;
            if (rotationAngle < 0f)
                rotationAngle += 360f;
            else if (rotationAngle >= 360f) rotationAngle -= 360f;
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

        private void AdjustPosition(float xDelta, float zDelta)
        {
            var direction =
                transform.localRotation *
                new Vector3(xDelta, 0f, zDelta).normalized;
            var damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            var distance =
                Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
                damping * Time.deltaTime;

            var position = transform.localPosition;
            position += direction * distance;
            transform.localPosition = ClampPosition(position);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            var xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
            position.x = Mathf.Clamp(position.x, 0f, xMax);

            var zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);

            return position;
        }
    }
}