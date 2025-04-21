using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    /// <summary>
    /// this is where all the public functions for building dialogue sequences live. should this even be a class?
    /// </summary>
    public class TalkCommands
    {
        /// <summary>
        /// a sequence of lines performed by the talker. even though the function is called "Talk" it can just be a character looking at another character or scratching their butt etc
        /// </summary>
        /// <param name="talker"></param>
        /// <param name="line"></param>
        public void Talk(List<Talker.Line> lines)
        {
            foreach (Talker.Line line in lines) 
            {
                line.TalkerInstance.Talk(line);
            }
        }
        
        public void PinSubtitle(Subtitle subtitle, Vector3 position)
        {
            // TODO: when this is called, subtitles will now appear in a specific spot in screen or world space
        }

        public void UnpinSubtitle(Subtitle subtitle)
        {
            // TODO: clear subtitle pin
        }
    }
}