#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine.Serialization;

namespace WalkAndTalk
{
    /// <summary>
    /// this is a fun one - press a button to start recording yourself walking around, then press it again to save your movements as a collection of keyframes!
    /// </summary>
    [ExecuteInEditMode]
    public class WalkRecorder : MonoBehaviour   // TODO: maybe I should make this a scriptable object or even an editor window or something? I don't see a reason this needs to exist as a GameObject in a scene
    {
        public bool PreviewIsPlaying { get; private set; }
        
        [SerializeField] private Transform target;
        [SerializeField] private KeyCode recordButton, localRecordButton;
        [SerializeField, Tooltip("save player position/rotation every __ seconds")] private float keyframeInterval = 0.1f;
        [SerializeField, Tooltip("recording file will be saved in this folder")] private string filepath = "Assets/WalkAndTalk/Data/WalkRecordings";

        // important variables
        private List<WalkRecording.PositionRotationKeyframe> recordedKeyframes = new List<WalkRecording.PositionRotationKeyframe>();
        private GameObject previewObject;
        private bool recording = false;
        
        // bleh variables
        private Coroutine previewCoroutineRuntime;
        private EditorCoroutine previewCoroutineEditor;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private float lastRecordTime = 0f;
        private bool lastRecordingWasLocal = false;

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                StartCoroutine(InitializeTargetAfterWait());
            }
        }

        private IEnumerator InitializeTargetAfterWait()
        {
            yield return new WaitForSeconds(0.1f);  // in case target is spawned in, we give it some time to appear
            InitializeTarget();
        }

        private void Update()
        {
            if (!Application.isPlaying || target == null)
            {
                return;
            }
            
            if (Input.GetKeyDown(recordButton))
            {
                ToggleRecording(false);
            }
            if (Input.GetKeyDown(localRecordButton))
            {
                ToggleRecording(true);
            }

            bool timeToRecordKeyframe = Time.time >= lastRecordTime + keyframeInterval;
            if (recording && timeToRecordKeyframe)
            {
                RecordKeyframe();
            }
        }
        
        private void ToggleRecording(bool pressedLocalButton)
        {
            if (recording)
            {
                recording = false;
                SaveRecording();
            }
            else
            {
                recording = true;
                lastRecordingWasLocal = pressedLocalButton;
                recordedKeyframes.Clear();
                startPosition = target.position;
                startRotation = target.rotation;

                StartCoroutine(ShowRecordingIndicator());
            }
        }
        
        private void RecordKeyframe()
        {
            Vector3 position = target.position;
            Vector3 rotation = target.eulerAngles;
            
            if (lastRecordingWasLocal)
            {
                position = startRotation * (position - startPosition);
                rotation = (Quaternion.Euler(rotation) * Quaternion.Inverse(startRotation)).eulerAngles;    // TODO: rotation doesn't look right in preview
            }
            
            WalkRecording.PositionRotationKeyframe keyframe = new WalkRecording.PositionRotationKeyframe
            {
                Position = position,
                Rotation = rotation
            };
            
            recordedKeyframes.Add(keyframe);
            lastRecordTime = Time.time;
        }
        
        private void SaveRecording()
        {
            WalkRecording walkRecording = ScriptableObject.CreateInstance<WalkRecording>();
            walkRecording.Local = lastRecordingWasLocal;
            walkRecording.KeyframeInterval = keyframeInterval;
            walkRecording.Keyframes = recordedKeyframes;
            
            string timestamp = DateTime.Now.ToString("yyyy-MMdd-HHmmss");
            string recordingName = $"{(lastRecordingWasLocal ? "Local" : "")}Recording_{timestamp}";
            
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            
            string fullPath = $"{filepath}/{recordingName}.asset";
            AssetDatabase.CreateAsset(walkRecording, fullPath);
            AssetDatabase.SaveAssets();
                
            Debug.Log($"Recording saved at {fullPath} with {recordedKeyframes.Count} keyframes.");
        }

        private IEnumerator ShowRecordingIndicator()
        {
            Debug.Log($"Recording " + (lastRecordingWasLocal ? "local movement" : "movement"));
            
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

            bool red = false;
            while (recording)
            {
                red = !red;
                for (int i = 0; i < renderers.Length; i++)
                {
                    for (int j = 0; j < renderers[i].materials.Length; j++)
                    {
                        if (renderers[i] != null)
                        {
                            renderers[i].materials[j].color = red ? Color.red : originalColors[i][j];
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
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

        public void PreviewRecording(WalkRecording recordingToPreview)
        {
            StopRecordingPreview();
            if (Application.isPlaying)
            {
                previewCoroutineRuntime = StartCoroutine(PlayRecordingPreview(recordingToPreview));
            }
            else
            {
                previewCoroutineEditor = EditorCoroutineUtility.StartCoroutineOwnerless(PlayRecordingPreview(recordingToPreview));
            }
            PreviewIsPlaying = true;
        }
        
        private IEnumerator PlayRecordingPreview(WalkRecording recordingToPreview)
        {
            if (recordingToPreview == null || recordingToPreview.Keyframes.Count == 0)
            {
                yield break;
            }
    
            previewObject = new GameObject("Recording Preview");
            previewObject.AddComponent<PositionGizmo>();
            Transform previewTransform = previewObject.transform;
    
            Vector3 previewStartPosition = target.position;
            Quaternion previewStartRotation = target.rotation;
    
            for (int i = 0; i < recordingToPreview.Keyframes.Count; i++)
            {
                WalkRecording.PositionRotationKeyframe keyframe = recordingToPreview.Keyframes[i];
        
                if (recordingToPreview.Local)
                {
                    previewTransform.rotation = previewStartRotation * Quaternion.Euler(keyframe.Rotation);
                    previewTransform.position = previewStartPosition + previewStartRotation * keyframe.Position;
                }
                else
                {
                    previewTransform.position = keyframe.Position;
                    previewTransform.rotation = Quaternion.Euler(keyframe.Rotation);
                }

                if (Application.isPlaying)
                {
                    yield return new WaitForSeconds(recordingToPreview.KeyframeInterval);
                }
                else
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                    yield return new EditorWaitForSeconds(recordingToPreview.KeyframeInterval);
                }
            }
    
            DestroyImmediate(previewObject.gameObject);
            PreviewIsPlaying = false;

            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();    // so that the preview button updates
        }

        public void StopRecordingPreview()
        {
            if (Application.isPlaying)
            {
                if (previewCoroutineRuntime != null)
                {
                    StopCoroutine(previewCoroutineRuntime);
                    previewCoroutineRuntime = null;
                }
            }
            else
            {
                if (previewCoroutineEditor != null)
                {
                    EditorCoroutineUtility.StopCoroutine(previewCoroutineEditor);
                    previewCoroutineEditor = null;
                }
            }
            
            if (previewObject != null)
            {
                DestroyImmediate(previewObject);
                previewObject = null;
            }
            
            PreviewIsPlaying = false;
        }

        public void InitializeTarget()
        {
            if (target == null)
            {
                target = transform;
            }
            else
            {
                if (PrefabUtility.GetPrefabAssetType(target) != PrefabAssetType.NotAPrefab)  // if target is a prefab
                {
                    string targetName = target.name;
                    target = GameObject.Find(targetName)?.transform;   // just find one with the same name
                    if (target == null)
                    {
                        target = GameObject.Find(targetName + "(Clone)")?.transform;   // or maybe it's spawned in
                    }
                    if (target == null)
                    {
                        Debug.LogWarning($"No '{targetName}' instance found, so you can't record its movement.");
                    }
                }
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
    
    /// <summary>
    /// just a lil blue triangle
    /// </summary>
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