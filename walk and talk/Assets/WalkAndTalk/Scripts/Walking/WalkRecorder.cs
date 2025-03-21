#if UNITY_EDITOR
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
        [SerializeField] private Transform target;  // TODO: if the user drags a prefab into this slot rather than a gameObject in the scene, the code should understand and account for this
        [SerializeField] private KeyCode recordButton, localRecordButton;
        [SerializeField, Tooltip("save player position/rotation every __ seconds")] private float keyframeInterval = 0.1f;
        [SerializeField] private string filepath = "Assets/WalkAndTalk/Data/WalkRecordings";

        private List<WalkRecording.PositionRotationKeyframe> recordedKeyframes = new List<WalkRecording.PositionRotationKeyframe>();
        private Coroutine previewCoroutine;
        private Vector3 startPosition;
        private Quaternion startRotation;
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
                startPosition = target.position;
                startRotation = target.rotation;

                StartCoroutine(ShowRecordingIndicator());
            }
            else
            {
                recording = false;
                SaveRecording();
            }
        }

        private IEnumerator ShowRecordingIndicator()
        {
            Debug.Log($"Recording " + (local ? "local movement" : "movement"));
            
            Color originalColor = target.GetComponentInChildren<Renderer>()?.material.color ?? Color.white;
            Renderer renderer = target.GetComponentInChildren<Renderer>();
    
            while (recording)
            {
                if (renderer != null)
                {
                    renderer.material.color = Time.time % 1 > 0.5f ? originalColor : Color.red;
                }
                yield return null;
            }
    
            if (renderer != null)
            {
                renderer.material.color = originalColor;
            }
        }
        
        private void SaveRecording()
        {
            WalkRecording walkRecording = ScriptableObject.CreateInstance<WalkRecording>();
            walkRecording.Local = local;
            walkRecording.KeyframeInterval = keyframeInterval;
            walkRecording.Keyframes = recordedKeyframes;
            
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string recordingName = $"{(local ? "Local" : "")}Recording_{timestamp}";
            
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            
            string fullPath = $"{filepath}/{recordingName}.asset";
            AssetDatabase.CreateAsset(walkRecording, fullPath);
            AssetDatabase.SaveAssets();
                
            Debug.Log($"Recording saved at {fullPath} with {recordedKeyframes.Count} keyframes.");
        }

        private void RecordKeyframe()
        {
            Vector3 position = target.position;
            Vector3 rotation = target.eulerAngles;
            
            if (local)
            {
                position = startRotation * (position - startPosition);
                rotation = (Quaternion.Euler(rotation) * Quaternion.Inverse(startRotation)).eulerAngles;
            }
            
            WalkRecording.PositionRotationKeyframe keyframe = new WalkRecording.PositionRotationKeyframe
            {
                Position = position,
                Rotation = rotation
            };
            
            recordedKeyframes.Add(keyframe);
            lastRecordTime = Time.time;
        }
        
        public void PreviewRecording(WalkRecording recording)
        {
            StopPreview();
            previewCoroutine = StartCoroutine(PlayRecordingPreview(recording));
        }

        public void StopPreview()
        {
            if (previewCoroutine != null)
            {
                StopCoroutine(previewCoroutine);
                previewCoroutine = null;
        
                GameObject previewObject = GameObject.Find("Recording Preview");    // TODO: is there a better way to do this?
                if (previewObject != null)
                {
                    DestroyImmediate(previewObject);
                }
            }
        }
        
        private IEnumerator PlayRecordingPreview(WalkRecording recording)
        {
            if (recording == null || recording.Keyframes.Count == 0)
            {
                yield break;
            }
    
            Transform previewObject = GameObject.CreatePrimitive(PrimitiveType.Cube).transform; // TODO: this should be a simple gizmo instead
            previewObject.name = "Recording Preview";
    
            Vector3 startPos = target.position;
            Quaternion startRot = target.rotation;
    
            for (int i = 0; i < recording.Keyframes.Count; i++)
            {
                WalkRecording.PositionRotationKeyframe keyframe = recording.Keyframes[i];
        
                if (recording.Local)
                {
                    previewObject.position = startPos + startRot * keyframe.Position;
                    previewObject.rotation = startRot * Quaternion.Euler(keyframe.Rotation);
                }
                else
                {
                    previewObject.position = keyframe.Position;
                    previewObject.rotation = Quaternion.Euler(keyframe.Rotation);
                }
        
                yield return new WaitForSeconds(recording.KeyframeInterval);    // TODO: this takes eons outside of Play mode (tested on a recording whose interval is 1sec), should maybe use a special editor coroutine if testing outside of runtime
            }
    
            DestroyImmediate(previewObject.gameObject);
        }
    }
}
#endif