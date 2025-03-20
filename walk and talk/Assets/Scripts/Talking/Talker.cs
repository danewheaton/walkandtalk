using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WalkAndTalk
{
    /// <summary>
    /// apply this component to gameObjects that talk
    /// </summary>
    public class Talker : MonoBehaviour
    {
        /* COMPATIBILITY
         * - I remember a popular old-school dialogue system that was simply called "dialogue system for unity" or something, maybe be compatible with that
         * - definitely compatible with ink
         * - definitely compatible with yarn/yarnspinner/whatever
         * - definitely compatible with whatever unity UI solutions are popular this week (textMeshPro, whatever "UI toolkit" is, legacy, whatever whatever bleh)
         * - should also have a standalone homebrew option for users who don't want any unity packages
         */
        
        [Serializable]
        public struct Line
        {
            // none of these are required, a line can be any combination of this stuff
            public string Speaker;  // TODO: should this be a string? all I know is I want the user to be able to select a character from a dropdown menu when they are populating line fields on a visual scripting node/inspector list element/etc, and I want new lines to be able to anticipate speaker based on previous lines and autopopulate accordingly
            public string SubtitleText; // TODO: should probably have a button that updates a localization database, and maybe another button similar to staging mode for walkers that allows the user to manually position subtitles in screen space or world space
            public AudioClip Audio; // TODO: there should be a button here to record audio from the user's microphone
            public AnimationClip Anim;  // TODO: should also have an inline timing field (float) right next to the clip
            public Transform MovementDestination;   // TODO: tried to make this a KeyValuePair of vectors for position and eulers, but didn't know how or even whether that is actually a good idea. also, should have an associated timing field
            public Transform LookTarget;    // TODO: this should actually be a dropdown of characters, with one entry just being "Transform" and changes the field to a Transform if selected. also, should have an associated timing field
            public bool RetainTarget;   // defaults to false, which means the character stops looking at the target after the line finishes
            public bool FaceBodyTowardTarget;   // TODO: should have an associated timing field
            public float TimeUntilNextLine;
            public UnityEvent BeginEvent;
            public UnityEvent CompleteEvent;
        }
        
        [SerializeField] private string characterName;    // TODO: there should be a button here that adds this string and a reference to this gameObject to a database somewhere, and throws a warning if the gameObject is not a prefab
        [Header("Defaults")]
        [SerializeField, Tooltip("this is optional, dragging an object into this slot will reset class defaults (but don't worry, you can change them again to override the global defaults)")] private TalkerGlobalDefaults globalDefaults;  // TODO: when this value changes, other inspector values should change too
        [SerializeField] private Color textColor = Color.gray;
        [SerializeField, Tooltip("95% of the time it makes more sense to let the global defaults handle this, unless you want everybody's subtitles to be wildly different for some reason")] private Subtitle subtitlePrefab;

        // TODO: somehow, we need the script to know where the talker's head is without requiring the user to manually tell us
        // TODO: emote system that supports states but doesn't require a state machine?
    }

    /// <summary>
    /// configurable default values for all talkers
    /// </summary>
    [CreateAssetMenu(fileName = "TalkerGlobalDefaults", menuName = "Walk and Talk/Talker Global Defaults")]
    public class TalkerGlobalDefaults : ScriptableObject
    {
        public Color DefaultTextColor = Color.gray;
        public Subtitle SubtitlePrefab;
    }
}