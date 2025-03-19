using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    /// <summary>
    /// this is a fun one - press a button to start recording yourself walking around, then press it again to save your movements as a collection of vectors!
    /// </summary>
    public class WalkRecorder : MonoBehaviour
    {
        [SerializeField] private KeyCode recordButton;
        [SerializeField, Tooltip("save player position/rotation every __ seconds")] private float snapshotInterval = .01f;

        private bool recording = false;

        // TODO: when a recording is made, save a scriptable object containing the positions and rotations for use in staging NPCs
    }

    public struct WalkRecording
    {
        public bool Local;  // if a recording is local, that means the positions (and rotations?) are saved as relative to the recorded walker's starting position (and rotation?) so that when applied to another walker, that walker follows the path described in the recording from wherever they are standing rather than walking to the exact position where the player started recording (an example of a local recording would be spinning around in a circle - we do not care where the player happened to be standing when they spun around in a circle, only that they spun around in a circle)
        public Dictionary<Vector3, Vector3> PositionsAndRotations;  // rotations are eulers, but maybe can be quaternions if there is a good reason for that
        public float SnapshotInterval;
    }
}