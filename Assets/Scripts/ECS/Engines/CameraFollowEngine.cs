using System.Collections;
using Svelto.ECS;
using Svelto.Tasks;
using UnityEngine;

namespace Hexagon.ECS.Camera
{
    //First step identify the entity type we want the engine to handle: CameraEntity
    //Second step name the engine according the behaviour and the entity: I.E.: CameraFollowTargetEngine
    //Third step start to write the code and create classes/fields as needed using refactoring tools 
    public class CameraFollowTargetEngine : MultiEntityViewsEngine<CameraEntityView, CameraTargetEntityView>
    {
        private readonly ITime _time;
        private CameraEntityView _cameraEntityView;
        private CameraTargetEntityView _cameraTargetEntityView;
        private readonly ITaskRoutine _taskRoutine;
        private readonly float moveSpeedMaxZoom = 400;
        private readonly float moveSpeedMinZoom = 100;
        private Vector3 newOffset;
        private Vector3 offset;
        private readonly float stickMaxZoom = 60;
        private readonly float stickMinZoom = 0;
        private float zoom = 1f;

        public CameraFollowTargetEngine(ITime time)
        {
            _time = time;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine().SetEnumerator(PhysicUpdate())
                .SetScheduler(StandardSchedulers.physicScheduler);
            _taskRoutine.Start();
        }

        protected override void Add(CameraEntityView entityView)
        {
            _cameraEntityView = entityView;
        }

        protected override void Remove(CameraEntityView entityView)
        {
            _taskRoutine.Stop();
            _cameraEntityView = null;
        }

        protected override void Add(CameraTargetEntityView entityView)
        {
            _cameraTargetEntityView = entityView;
            Initialize();
        }

        protected override void Remove(CameraTargetEntityView entityView)
        {
            _taskRoutine.Stop();
            _cameraTargetEntityView = null;
        }

        private void Initialize()
        {
            offset = new Vector3(0, 30, 30);
        }

        private void UpdateCamera()
        {

            var zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            zoom = Mathf.Clamp01(zoom - zoomDelta);

            if (Input.GetKey(KeyCode.Space))
            {
                CenterCamera();
                return;
            }

            var xDelta = Input.GetAxis("Horizontal");
            var zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f) AdjustPosition(xDelta, zDelta);
        }

        private void AdjustPosition(float xDelta, float zDelta)
        {
            var camera = _cameraEntityView.transformComponent;


            var direction =
                //camera.localRotation *
                new Vector3(xDelta, 0f, zDelta).normalized;
            var damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            var distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
                           damping * _time.deltaTime;

            camera.position -= direction * distance;
            //camera.position = ClampPosition(position);
        }

        private void CenterCamera()
        {
            var player = _cameraTargetEntityView.targetComponent;
            var camera = _cameraEntityView.transformComponent;


            var distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);

            var zoomResult = new Vector3(0, distance, distance / 2);

            offset = Quaternion.AngleAxis(Input.GetAxis("Rotation") * 4.0f, Vector3.up) *
                     offset; //(offset + zoomResult);

            camera.position = player.position + offset + zoomResult;
            _cameraEntityView.cameraComponent.LookAt(player.position + new Vector3(0, 7, 0));
        }

        private IEnumerator PhysicUpdate()
        {
            while (_cameraEntityView == null || _cameraTargetEntityView == null)
                yield return null; //skip a frame    

            while (true)
            {
                UpdateCamera();
                yield return null;
            }
        }
    }
}