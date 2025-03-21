using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    /// <summary>
    /// apply this component to gameObjects that walk
    /// </summary>
    public class Walker : MonoBehaviour
    {
        /* COMPATIBILITY
         * - should be compatible with unity's animation rigging package
         * - should be compatible with unity's starter assets package
         * - should also have a standalone homebrew option for users who don't want any unity packages
         */
        
        public enum Speed { Sneak, Stroll, Walk, Stride, Jog, Run, Sprint }
        
        [Serializable]
        public struct FootstepActions
        {
            public enum SurfaceType { Default, Dirt, Grass, Wood, RoughStone, SmoothStone, Tile, Carpet } // TODO: is there a built-in unity solution for this? if so, I'll have to figure out how to balance interoperability and the convenience of a one-stop shop

            public SurfaceType Surface;
            public AudioClip Audio;
        }

        [Header("Defaults")]
        [SerializeField, Tooltip("this is optional, dragging an object into this slot will reset default speed etc (but don't worry, you can change them again to override the global defaults)")] private WalkerGlobalDefaults globalDefaults;  // TODO: when this value changes, other inspector values should change too
        [SerializeField] private Speed defaultSpeed = Speed.Walk;
        [SerializeField] private List<FootstepActions> footstepActions;

        public Speed DefaultSpeed => defaultSpeed;
        public Speed CurrentSpeed { get; private set; }
        
        // TODO: locomotion “just works” both with nav mesh and without nav mesh, super super super easy for inexperienced designers to pick up and get working
        // TODO: this script could maybe also handle stuff like animation/ik blending and layering
        
        /* TODO: might try to put together a nuanced and interesting speed override system that handles speed in different contexts with configurable priorities:
         * - trigger colliders can set default speed per area
         * - per-character default speeds for different area types
         * - maybe move command can override speed under some conditions but not others
         * - speed can be overridden directly via visual scripting/action sequence/etc, BUT has a forced time limit after which character will revert to default speed (time limit can be circumvented by passing 0 as arg, but that is discouraged)
         * - optional exhaustion system?
         */
    }
    
    /// <summary>
    /// configurable default values for all walkers
    /// </summary>
    [CreateAssetMenu(fileName = "WalkerGlobalDefaults", menuName = "Walk and Talk/Walker Global Defaults")]
    public class WalkerGlobalDefaults : ScriptableObject
    {
        public Walker.Speed DefaultSpeed = Walker.Speed.Walk;
        [Tooltip("things that happen when the walker's foot hits the ground")] public List<Walker.FootstepActions> FootstepActions;
    }
}