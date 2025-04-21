using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WalkAndTalk
{
    /// <summary>
    /// apply this component to a gameObject that talkers can use to show a line's text
    /// </summary>
    public class Subtitle : MonoBehaviour
    {
        public enum Styles { Default, Feed, NearTalker }

        public GameObject NameText => nameText;
        public GameObject LineText => lineText;

        [SerializeField, Tooltip("default = appears in a predictable spot, typically bottom of screen, like a movie\n\nfeed = in a scrolling feed, like Disco Elysium\n\nnear talker = offset from character, like Monkey Island")] private Styles style = Styles.Default;
        [SerializeField] private GameObject nameText, lineText;

        // TODO: click a button in the inspector to generate and assign UI gameObjects appropriate to the style if nameText and lineText are unassigned
    }
}