using UnityEngine;
using UnityEditor;

namespace Alteruna
{
    namespace Trinity
    {
        [CustomEditor(typeof(AlterunaTrinity))]
        public class AlterunaTrinityEditor : Editor
        {
            public GUISkin CustomSkin;
            protected AlterunaTrinity mTarget;

            private void Awake()
            {
                mTarget = (AlterunaTrinity)target;
            }

            public override void OnInspectorGUI()
            {
                GUIStyle centered = new GUIStyle(GUI.skin.label);
                centered.alignment = TextAnchor.MiddleCenter;
                centered.fontStyle = FontStyle.Bold;
                centered.fontSize = 18;
                EditorGUILayout.LabelField("Alteruna Trinity", centered, GUILayout.ExpandWidth(true), GUILayout.Height(30));

                centered.alignment = TextAnchor.MiddleCenter;
                centered.fontStyle = FontStyle.Normal;
                centered.fontSize = 13;
                centered.padding = new RectOffset(0, 0, 0, 0);
                EditorGUILayout.LabelField("Username: " + mTarget.ClientName, centered, GUILayout.ExpandWidth(true), GUILayout.Height(15));

                if (mTarget.InPlayroom)
                {
                    EditorGUILayout.LabelField("In Playroom | User Index: " + mTarget.UserIndex, centered, GUILayout.ExpandWidth(true), GUILayout.Height(15));
                }
                else
                {
                    EditorGUILayout.LabelField("Not in a Playroom", centered, GUILayout.ExpandWidth(true), GUILayout.Height(15));
                }
                
                SerializedProperty iter = serializedObject.GetIterator();
                iter.NextVisible(true);
                while (iter.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(iter, true);
                }

                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
