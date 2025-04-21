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
            public Talker TalkerInstance;  // TODO: should this be a different data type? all I know is I want the user to be able to select a character from a dropdown menu when they are populating line fields on a visual scripting node/inspector list element/etc, and I want new lines to be able to anticipate speaker based on previous lines and autopopulate accordingly
            public string SubtitleText; // TODO: should probably have a button that updates a localization database, and maybe another button similar to staging mode for walkers that allows the user to manually position subtitles in screen space or world space
            public AudioClip Audio; // TODO: there should be a button here to record audio from the user's microphone
            public AnimationClip Anim;  // TODO: should also have an inline timing field (float) right next to the clip
            public Transform MovementDestination;   // TODO: tried to make this a KeyValuePair of vectors for position and eulers, but didn't know how or even whether that is actually a good idea. also, should have an associated timing field
            public Transform LookTarget;    // TODO: this should actually be a dropdown of characters, with one entry just being "Transform" and changes the field to a Transform if selected. also, should have an associated timing field
            public bool RetainTarget;   // defaults to false, which means the character stops looking at the target after the line finishes
            public bool FaceBodyTowardTarget;   // TODO: should have an associated timing field
            public float WaitTime;  // can be negative, in which case next line overlaps
            public List<UnityEvent> BeginEvents;
            public List<UnityEvent> CompleteEvents;
        }

        public string TalkerName => talkerName;
        
        [SerializeField] private string talkerName;    // TODO: there should be a button here that adds this string and a reference to this gameObject to a database somewhere, and throws a warning if the gameObject is not a prefab
        [Header("Defaults")]
        [SerializeField, Tooltip("this is optional, dragging an object into this slot will reset class defaults (but don't worry, you can change them again to override the global defaults)")] private TalkerGlobalDefaults globalDefaults;  // TODO: when this value changes, other inspector values should change too
        [SerializeField] private Color textColor = Color.gray;
        [SerializeField, Tooltip("95% of the time it makes more sense to let the global defaults handle this, unless you want everybody's subtitles to be wildly different for some reason")] private Subtitle subtitlePrefab;
        [SerializeField] private Transform head;

        // TODO: somehow, we need the script to know where the talker's head is without requiring the user to manually tell us
        // TODO: emote system that supports states but doesn't require a state machine?

        public void Talk(Line line)
        {
            /* HOW IT SHOULD WORK
             * - check for subtitle in global defaults, then component
             *  - if nothing, skip subtitle text display
             *  - otherwise:
             *      - put talkerName in name text field
             *      - put line.SubtitleText in line text field
             *      - if default or feed subtitle positioning, color the name text field textColor
             *      - or if nearTalker subtitle positioning, omit name text field and color line text field instead
             *      - if there's no talkerName, just don't show name text field
             *      - if talkerName is unrecognized, use global defaults
             *      - TODO: maybe think of a cool system for handling fun juicy expressive text effects like in Katana Zero
             * - check for audio file in line
             *  - if nothing, skip audio and keep the line onscreen based on number of characters (letters/numbers/symbols/etc) in the line plus WaitTime. TODO: should I have a system for playing silly little noises when a character speaks? game devs love using silly little noises for dialogue
             *  - otherwise:
             *      - if there's no AudioSource component on the talker, add one and configure its settings based on global defaults
             *      - plug the clip into the AudioSource
             *      - the line lasts for clip length plus WaitTime
             * - check for anim file in line
             *  - if anim:
             *      - if there's no Animator component on the talker, don't bother trying to add and configure one on the fly. just throw an error
             *      - play the animation, with specified timing
             *      - some anims should lead into a bespoke idle state, which can have its own emotes and exit anim. TODO: not sure how I want to tackle this, other than it would be cool to somehow sidestep using a state machine (or at least a bloated, complex state machine)
             * - check for movement destination transform in line
             *  - if destination:
             *      - if no walker in talker's hierarchy, just throw a warning that there's no walker. don't try to add one
             *      - otherwise, the walker should navigate to that position and rotation, with specified timing
             * - check for look target transform in line
             *  - if target:
             *      - if no reference to talker's head, just throw a warning that we can't turn the walker's head because we don't know where it is
             *      - if target is behind the talker or otherwise outside the natural range of the talker's neck movement, do nothing
             *      - otherwise, turn talker's head toward target constantly
             *      - if FaceBodyTowardTarget, shuffle feet and face talker's body toward target as well, with specified timing. this should happen once, not constantly
             *      - if RetainTarget, keep looking at the target
             *      - otherwise, stop looking after the line finishes
             *      - the part of the code handling looking should be easy to find and edit, since I assume different games might want to do this differently (lerp, anim, stop-motion, etc)
             * - fire any begin events
             * - when processes have finished, fire any complete events
             */
        }
    }
}