using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    public class WalkEditor
    {
        /* I have no idea how to code editor stuff, so for right now I am just using this class to think through my feature wishlist
         *
         * STAGING MODE
         * - on visual scripting nodes and inspector list elements that call WalkCommands.Move() or similar, have a button that says "staging mode"
         * - when this button is clicked, a gizmo should appear in the middle of the scene view, on a navigable surface
         * - it should be MAXIMALLY easy to see and adjust the position and rotation of this gizmo using just the mouse
         * - the gizmo is either locked to or points at navigable surfaces, to indicate where a walker can stand
         * - if we know the character we are staging, its model is part of the gizmo in an idle pose (to preview what it will look like during gameplay)
         * - when the button is pressed again, the gizmo's position and rotation are saved to the node or list element or whatever
         */
    }
}