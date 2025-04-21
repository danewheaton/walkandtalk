using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    /// <summary>
    /// configurable default values for all talkers
    /// </summary>
    [CreateAssetMenu(fileName = "TalkerGlobalDefaults", menuName = "Walk and Talk/Talker Global Defaults")]
    public class TalkerGlobalDefaults : ScriptableObject
    {
        public Color DefaultTextColor = Color.gray;
        public Subtitle SubtitlePrefab;
        [Tooltip("if this is unchecked, player will have to click to continue to the next line")] public bool AutoplayLines = true;
        // TODO: AudioSource settings
    }
}