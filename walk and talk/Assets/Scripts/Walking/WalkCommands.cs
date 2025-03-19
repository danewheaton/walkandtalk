using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    /// <summary>
    /// this is where all the public functions for staging characters live. should this even be a class?
    /// </summary>
    public class WalkCommands
    {
        public void Move(Walker walker, Vector3 destinationPosition, Vector3? destinationDirection = null, Walker.Speed? speed = null)
        {
            // if walker is being controlled by the player, disable player control
            // make walker walk to destinationPosition, at speed
            // if no speed specified, use walker.CurrentSpeed
            // once at destinationPosition, face destinationDirection (if provided)
            // if walker was being controlled by the player, re-enable player control
        }
        
        public void Move(Walker walker, WalkRecording recording)
        {
            // if walker is being controlled by the player, disable player control
            // make walker follow the movements in the recording
            // if walker was being controlled by the player, re-enable player control after the walker reaches the final position/rotation in the recording
        }
    }
}