using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MRK
{
    public class OrbitCameraBehaviour : CameraBehaviour
    {
        private object _rotTween;
        private Tween _distTween;
        private float _startTime;
        private bool _down;

        public OrbitCameraBehaviour(Transform moon, Camera camera) : base(moon, camera)
        {
        }

        public override void OnEnabled()
        {
            _startTime = Time.time;

            //lerp into sample pos rot
            var smp = GetSamplePosRot();

            _camera.transform.DOMove(smp.Item1, 0.5f);
            _camera.transform.DORotate(smp.Item2, 0.5f);
        }

        public override void OnDisabled()
        {
        }

        public override void Update()
        {
            if (Time.time - _startTime > 0.5f)
            {
                if (Input.mouseScrollDelta != Vector2.zero)
                {
                    ProcessZoomScroll(Input.GetAxis("Mouse ScrollWheel") * 300f);
                }

                UpdateTransform();
            }
        }

        public override void OnGUI()
        {
            var evt = Event.current;
            if (evt == null) return;

            switch (evt.type)
            {
                case EventType.MouseDown:
                    if (!EventSystem.current.IsPointerOverGameObject(-1))
                    {
                        _down = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (_down)
                    {
                        ProcessRotation(evt.delta);
                    }
                    break;

                case EventType.MouseUp:
                    _down = false;
                    break;
            }
        }

        public (Vector3, Vector3) GetSamplePosRot()
        {
            Quaternion rotation = Quaternion.Euler(OrbitData.CurrentRotation.y, OrbitData.CurrentRotation.x, 0);

            Vector3 negDistance = new Vector3(0f, 0f, -OrbitData.CurrentDistance);
            Vector3 position = (rotation * negDistance) + _moon.position;

            return (position, rotation.eulerAngles);
        }

        private void ProcessRotation(Vector3 delta, bool withTween = true, bool withDelta = true)
        {
            float factor = Mathf.Clamp01((OrbitData.CurrentDistance / OrbitData.MaximumDistance) + 0.5f);
            OrbitData.TargetRotation.x += delta.x * (withDelta ? Time.deltaTime : 1f) * factor * 75f;
            OrbitData.TargetRotation.y += delta.y * (withDelta ? Time.deltaTime : 1f) * factor * 75f;

            //_targetRotation.y = ClampAngle(_targetRotation.y, -80f, 80f);

            if (_rotTween != null)
            {
                DOTween.Kill(_rotTween);
            }

            if (withTween)
            {
                _rotTween = DOTween.To(() => OrbitData.CurrentRotation, x => OrbitData.CurrentRotation = x, OrbitData.TargetRotation, 0.4f)
                    .SetEase(Ease.OutQuint);
            }
        }

        private void ProcessZoomInternal(float rawDelta)
        {
            OrbitData.TargetDistance -= rawDelta; // * Time.deltaTime;

            OrbitData.TargetDistance = Mathf.Clamp(OrbitData.TargetDistance, OrbitData.MinimumDistance, OrbitData.MaximumDistance);

            if (_distTween != null)
            {
                DOTween.Kill(_distTween.id);
            }

            _distTween = DOTween.To(() => OrbitData.CurrentDistance, x => OrbitData.CurrentDistance = x, OrbitData.TargetDistance, 0.2f).SetEase(Ease.OutQuint);
        }

        private void ProcessZoomScroll(float delta)
        {
            ProcessZoomInternal(delta);
        }

        private void UpdateTransform()
        {
            Quaternion rotation = Quaternion.Euler(OrbitData.CurrentRotation.y, OrbitData.CurrentRotation.x, 0);

            Vector3 negDistance = new Vector3(0f, 0f, -OrbitData.CurrentDistance);
            Vector3 position = (rotation * negDistance) + _moon.position;

            _camera.transform.rotation = rotation;
            _camera.transform.position = position;
        }
    }
}
