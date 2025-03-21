using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    [CreateAssetMenu(fileName = "WalkRecording", menuName = "Walk and Talk/Walk Recording")]
    public class WalkRecording : ScriptableObject
    {
        [Serializable]
        public struct PositionRotationKeyframe
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }
        
        public bool Local = false;  // if a recording is local, that means the positions and rotations are saved as relative to the recorded walker's starting point so that when applied to another walker, that walker follows the path described in the recording from wherever they are standing rather than walking to the exact position where the player started recording (an example of a local recording would be spinning around in a circle - we do not care where the player happened to be standing when they spun around in a circle, only that they spun around in a circle)
        public float KeyframeInterval = 0.1f;
        public List<PositionRotationKeyframe> Keyframes;
    }
}