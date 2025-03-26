#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WalkAndTalk
{
    [CustomEditor(typeof(WalkRecording))]
    public class WalkRecordingEditor : Editor
    {
        private static bool previewIsPlaying = false;
        private static WalkRecorder activeRecorder = null;
        
        public override void OnInspectorGUI()
        {
            WalkRecording recording = (WalkRecording)target;
            
            string buttonText = previewIsPlaying ? "Stop Preview" : "Preview in Scene"; // TODO: this continues to show "Preview in Scene" after the preview finishes
            if (GUILayout.Button(buttonText))
            {
                if (previewIsPlaying)
                {
                    if (activeRecorder != null)
                    {
                        activeRecorder.StopPreview();
                        activeRecorder = null;
                    }
                    previewIsPlaying = false;
                }
                else
                {
                    WalkRecorder recorder = FindObjectOfType<WalkRecorder>();
                    if (recorder == null)
                    {
                        GameObject previewObject = new GameObject("Recording Preview");
                        recorder = previewObject.AddComponent<WalkRecorder>();
                    }
                
                    recorder.PreviewRecording(recording);
                    activeRecorder = recorder;
                    previewIsPlaying = true;
                }
            }
            
            DrawDefaultInspector();
        }
    }
}
#endif