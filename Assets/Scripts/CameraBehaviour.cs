using UnityEngine;

namespace MRK
{
    public abstract class CameraBehaviour
    {
        protected Transform _moon;
        protected Camera _camera;

        public static OrbitData OrbitData { get; private set; } = new OrbitData();

        public CameraBehaviour(Transform moon, Camera camera)
        {
            _moon = moon;
            _camera = camera;
        }

        public virtual void OnEnabled()
        {
        }

        public virtual void OnDisabled()
        {
        }


        public abstract void Update();

        public virtual void OnGUI()
        {
        }
    }
}
