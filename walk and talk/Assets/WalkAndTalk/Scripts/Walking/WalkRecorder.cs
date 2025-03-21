using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WalkAndTalk
{
    /// <summary>
    /// this is a fun one - press a button to start recording yourself walking around, then press it again to save your movements as a collection of keyframes!
    /// </summary>
    public class WalkRecorder : MonoBehaviour
    {
        private static int numRecordings = 0;  // TODO: placeholder, not sure yet how I want to auto-name recording files
        
#if UNITY_EDITOR
        [SerializeField] private Transform target;  // TODO: make this work with prefabs too
        [SerializeField] private KeyCode recordButton, localRecordButton;
        [SerializeField, Tooltip("save player position/rotation every __ seconds")] private float keyframeInterval = 0.1f;
        [SerializeField] private string filepath = "Assets/WalkAndTalk/Data/WalkRecordings";

        private List<WalkRecording.PositionRotationKeyframe> recordedKeyframes = new List<WalkRecording.PositionRotationKeyframe>();
        private float lastRecordTime = 0f;
        private bool recording = false, local = false;

        private void Awake()
        {
            if (target == null)
            {
                target = transform;
            }
        }

        private void Update()
        {
            // TODO: these shouldn't conflict with each other, and starting one while the other is recording should abort the other
            if (Input.GetKeyDown(recordButton))
            {
                local = false;
                ToggleRecording();
            }
            if (Input.GetKeyDown(localRecordButton))
            {
                local = true;
                ToggleRecording();
            }

            bool readyToRecordKeyframe = Time.time >= lastRecordTime + keyframeInterval;
            if (recording && readyToRecordKeyframe)
            {
                RecordKeyframe();
            }
        }
        
        private void ToggleRecording()
        {
            if (!recording)
            {
                recording = true;
                recordedKeyframes.Clear();
                // TODO: do something more in-the-player's face than a debug message to visually indicate that the player's movements are being recorded
            }
            else
            {
                recording = false;
                WalkRecording walkRecording = ScriptableObject.CreateInstance<WalkRecording>();
                walkRecording.Local = local;
                walkRecording.KeyframeInterval = keyframeInterval;
                walkRecording.Keyframes = recordedKeyframes;
                AssetDatabase.CreateAsset(walkRecording, filepath + "/WalkRecording" + numRecordings + ".asset");
                AssetDatabase.SaveAssets();
                numRecordings++;
                
                Debug.Log($"Recording stopped. Captured {recordedKeyframes.Count} keyframes.");
            }
        }

        private void RecordKeyframe()
        {
            WalkRecording.PositionRotationKeyframe keyframe = new WalkRecording.PositionRotationKeyframe
            {
                // TODO: add logic for saving local values too
                Position = target.position,
                Rotation = target.eulerAngles
            };
            
            recordedKeyframes.Add(keyframe);
            lastRecordTime = Time.time;
        }
#endif
    }
}