using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class AnimationStepDTO
    {
        public float framesPerStep = 30f;
        public Vector3 lowerBodyRotation;
        public Vector3 upperLeftLegRotation;
        public Vector3 lowerLeftLegRotation;
        public Vector3 upperRightLegRotation;
        public Vector3 lowerRightLegRotation;
        public Vector3 upperBodyRotation;
        public Vector3 upperLeftArmRotation;
        public Vector3 lowerLeftArmRotation;
        public Vector3 upperRightArmRotation;
        public Vector3 lowerRightArmRotation;
        public Vector3 headRotation;
    }
}