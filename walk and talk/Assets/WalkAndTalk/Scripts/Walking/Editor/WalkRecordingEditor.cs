#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WalkAndTalk
{
    [CustomEditor(typeof(WalkRecording))]
    public class WalkRecordingEditor : Editor
    {
        private static WalkRecorder activeRecorder = null;
        private static bool previewIsPlaying = false;
        
        public override void OnInspectorGUI()
        {
            previewIsPlaying = false;
            if (activeRecorder != null)
            {
                previewIsPlaying = activeRecorder.PreviewIsPlaying;
            }
            
            WalkRecording recording = (WalkRecording)target;
            string buttonText = previewIsPlaying ? "Stop Preview" : "Preview in Scene";
            
            if (GUILayout.Button(buttonText))
            {
                if (previewIsPlaying)
                {
                    if (activeRecorder != null)
                    {
                        activeRecorder.StopRecordingPreview();
                        activeRecorder = null;
                    }
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
                }
            }
            
            DrawDefaultInspector();
        }
    }
}
#endif