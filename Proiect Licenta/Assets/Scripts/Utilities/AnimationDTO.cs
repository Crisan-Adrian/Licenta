using System;
using System.Collections.Generic;

namespace Utilities
{
    [Serializable]
    public class AnimationDTO
    {
        public string animationName;
        public List<AnimationStepDTO> steps;

        public AnimationDTO()
        {
            steps = new List<AnimationStepDTO>();
        }
    }
}