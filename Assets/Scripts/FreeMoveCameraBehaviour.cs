using SpaceGraphicsToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MRK
{
    public class FreeMoveCameraBehaviour : CameraBehaviour
    {
        private SgtCameraLook _cameraLook;
        private SgtCameraMove _cameraMove;

        public FreeMoveCameraBehaviour(Transform moon, Camera camera) : base(moon, camera)
        {
            //this constructor is called in the main thread!!!

            _cameraLook = camera.GetComponent<SgtCameraLook>();
            _cameraMove = camera.GetComponent<SgtCameraMove>();

            //by default disable components
            SetComponentsState(false);
        }

        public override void OnEnabled()
        {
            //enable components
            SetComponentsState(true);
        }

        public override void OnDisabled()
        {
            //disbale components
            SetComponentsState(false);
        }

        public override void Update()
        {

        }

        private void SetComponentsState(bool enabled)
        {
            _cameraLook.enabled = enabled;
            _cameraMove.enabled = enabled;
        }
    }
}
