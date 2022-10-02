using UnityEngine;

namespace MRK
{
    public class OrbitData
    {
        public Vector2 TargetRotation;
        public Vector2 CurrentRotation;
        public float CurrentDistance = 5000f;
        public float TargetDistance = 5000f;
        public float MinimumDistance = 1000f;
        public float MaximumDistance = 7000f;
    }
}
