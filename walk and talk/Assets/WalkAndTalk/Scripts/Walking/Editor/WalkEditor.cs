using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    public class WalkEditor
    {
        /* feature wishlist:
         *
         * STAGING MODE
         * - on visual scripting nodes and inspector list elements that call WalkCommands.Move() or similar, have a button that says "staging mode"
         * - when this button is clicked, a gizmo should appear in the middle of the scene view, on a navigable surface
         * - it should be MAXIMALLY easy to see and adjust the position and rotation of this gizmo using just the mouse
         * - the gizmo is either locked to or points at navigable surfaces, to indicate where a walker can stand
         * - if we know the character we are staging, its model is part of the gizmo in an idle pose (to preview what it will look like during gameplay)
         * - when the button is pressed again, the gizmo's position and rotation are saved to the node or list element or whatever
         *
         * EDITOR COROUTINES
         * - currently, stuff written to happen over multiple frames, like coroutines on the WalkRecorder, don't work the same outside of runtime
         * - would be nice if things that take 1 second at runtime also take 1 second outside of runtime as well
         * - this .cs file named "WalkEditor" does not have to just be a class with the same name, can contain multiple util classes instead
         *
         * GIZMOS
         * - user should be able to see gizmos that clearly show useful info about character positioning without requiring any special fields or asset references
         * - gizmo system should "just know" about any character models the user has assigned, and plop them in when the user is testing stuff
         * - and in any case, gizmos should have clear handles and directional arrows/indicators etc
         */
    }
}