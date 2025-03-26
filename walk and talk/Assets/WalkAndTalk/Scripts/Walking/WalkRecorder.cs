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
        public bool PreviewIsPlaying { get; private set; }
        
        [SerializeField] private Transform target;  // TODO: if the user drags a prefab into this slot rather than a gameObject in the scene, the code should understand to look for an instance at runtime
        [SerializeField] private KeyCode recordButton, localRecordButton;   // TODO: this should also account for the new input system
        [SerializeField, Tooltip("save player position/rotation every __ seconds")] private float keyframeInterval = 0.1f;
        [SerializeField, Tooltip("recording file will be saved in this folder")] private string filepath = "Assets/WalkAndTalk/Data/WalkRecordings";

        private List<WalkRecording.PositionRotationKeyframe> recordedKeyframes = new List<WalkRecording.PositionRotationKeyframe>();
        private GameObject previewObject;
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
            if (recording)
            {
                recording = false;
                SaveRecording();
            }
            else
            {
                recording = true;
                recordedKeyframes.Clear();
                startPosition = target.position;
                startRotation = target.rotation;

                StartCoroutine(ShowRecordingIndicator());
            }
        }

        private IEnumerator ShowRecordingIndicator()
        {
            Debug.Log($"Recording " + (local ? "local movement" : "movement"));
            
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            Color[][] originalColors = new Color[renderers.Length][];
            
            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = new Color[renderers[i].materials.Length];
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    originalColors[i][j] = renderers[i].materials[j].color;
                }
            }
            
            while (recording)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    for (int j = 0; j < renderers[i].materials.Length; j++)
                    {
                        if (renderers[i] != null)
                        {
                            renderers[i].materials[j].color = Time.time % 1 > 0.5f ? originalColors[i][j] : Color.red;
                        }
                    }
                }
                yield return null;
            }
            
            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    if (renderers[i] != null)
                    {
                        renderers[i].materials[j].color = originalColors[i][j];
                    }
                }
            }
        }
        
        private void SaveRecording()
        {
            WalkRecording walkRecording = ScriptableObject.CreateInstance<WalkRecording>();
            walkRecording.Local = local;    // TODO: if I was recording a global clip, and then click the local recording button, the file is saved as local. expected behavior is it is saved as global
            walkRecording.KeyframeInterval = keyframeInterval;
            walkRecording.Keyframes = recordedKeyframes;
            
            string timestamp = DateTime.Now.ToString("yyyy-MMdd-HHmmss");
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
            PreviewIsPlaying = true;
        }

        public void StopPreview()
        {
            if (previewCoroutine != null)
            {
                StopCoroutine(previewCoroutine);
                previewCoroutine = null;
        
                if (previewObject != null)
                {
                    DestroyImmediate(previewObject);
                    previewObject = null;
                }
            }
            PreviewIsPlaying = false;
        }
        
        private IEnumerator PlayRecordingPreview(WalkRecording recording)
        {
            if (recording == null || recording.Keyframes.Count == 0)
            {
                yield break;
            }
    
            previewObject = new GameObject("Recording Preview");
            previewObject.AddComponent<PositionGizmo>();
            Transform previewTransform = previewObject.transform;
    
            Vector3 startPos = target.position;
            Quaternion startRot = target.rotation;
    
            for (int i = 0; i < recording.Keyframes.Count; i++)
            {
                WalkRecording.PositionRotationKeyframe keyframe = recording.Keyframes[i];
        
                if (recording.Local)
                {
                    previewTransform.position = startPos + startRot * keyframe.Position;
                    previewTransform.rotation = startRot * Quaternion.Euler(keyframe.Rotation);
                }
                else
                {
                    previewTransform.position = keyframe.Position;
                    previewTransform.rotation = Quaternion.Euler(keyframe.Rotation);
                }

                if (Application.isPlaying)
                {
                    yield return new WaitForSeconds(recording.KeyframeInterval);
                }
                else
                {
                    // TODO: this doesn't work (and maybe can't work? I don't know if it's possible to see something happening smoothly in scene view over time while the game isn't running)
                    float startTime = (float)EditorApplication.timeSinceStartup;
                    while ((float)EditorApplication.timeSinceStartup - startTime < recording.KeyframeInterval)
                    {
                        yield return null;
                        SceneView.RepaintAll();
                    }
                }
            }
    
            DestroyImmediate(previewObject.gameObject);
            PreviewIsPlaying = false;
        }
    }
    
    public class PositionGizmo : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Vector3 forward = transform.forward * 0.5f;
            Vector3 right = transform.right * 0.15f;
            Vector3 back = -transform.forward * 0.15f;

            Vector3 slightlyAboveGround = transform.position + Vector3.up * 0.1f;
            Vector3 tip = slightlyAboveGround + forward;
            Vector3 leftBase = slightlyAboveGround + back - right;
            Vector3 rightBase = slightlyAboveGround + back + right;
            
            Gizmos.DrawLine(tip, leftBase);
            Gizmos.DrawLine(tip, rightBase);
            Gizmos.DrawLine(leftBase, rightBase);
        }
    }
}
#endif