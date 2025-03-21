#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WalkAndTalk
{
    [CustomEditor(typeof(WalkRecording))]
    public class WalkRecordingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WalkRecording recording = (WalkRecording)target;
            
            if (GUILayout.Button("Preview in Scene"))
            {
                // TODO: this button should change to "stop preview" when preview is playing
                
                WalkRecorder recorder = FindObjectOfType<WalkRecorder>();
                if (recorder == null)
                {
                    GameObject go = new GameObject("Recording Preview");
                    recorder = go.AddComponent<WalkRecorder>();
                    // TODO: if we did this, we should delete it when the preview ends
                }
                
                recorder.PreviewRecording(recording);
            }
            
            DrawDefaultInspector();
        }
    }
}
#endif