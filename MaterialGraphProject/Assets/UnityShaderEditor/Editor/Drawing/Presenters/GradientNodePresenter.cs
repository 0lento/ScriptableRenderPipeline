using System;
using System.Collections.Generic;
using RMGUI.GraphView;
using UnityEditor.Graphing.Drawing;
using UnityEngine;


namespace UnityEditor.MaterialGraph.Drawing
{
    [Serializable]
    class GradientContolPresenter : GraphControlPresenter
    {
        [SerializeField]
        private GradientObject gradientobj;

        [SerializeField]
        private SerializedObject hserializedObject;

        [SerializeField]
        private SerializedProperty hcolorGradient;

        private UnityEngine.MaterialGraph.GradientNode prevnode;

        private string prevWindow = "";

        public override void OnGUIHandler()
        {
            base.OnGUIHandler();

            var cNode = node as UnityEngine.MaterialGraph.GradientNode;
            if (cNode == null)
                return;

            if(gradientobj == null || prevnode != cNode)
            {
                prevnode = cNode;
                gradientobj = new GradientObject();
                if (cNode.gradient != null)
                {
                    gradientobj.gradient = cNode.gradient;
                }

                hserializedObject = new SerializedObject(gradientobj);
                hcolorGradient = hserializedObject.FindProperty("gradient");
            }

            EditorGUILayout.PropertyField(hcolorGradient, true, null);
            hserializedObject.ApplyModifiedProperties();
            cNode.gradient = gradientobj.gradient;

            Event e = Event.current;

            if (EditorWindow.focusedWindow != null && prevWindow != EditorWindow.focusedWindow.ToString() && EditorWindow.focusedWindow.ToString() != "(UnityEditor.GradientPicker)")
            {
                cNode.UpdateGradient();
                prevWindow = EditorWindow.focusedWindow.ToString();
                Debug.Log("Update Gradient Shader");
            }
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + 10 * EditorGUIUtility.standardVerticalSpacing;
        }

    }

    [Serializable]
    public class GradientNodePresenter : MaterialNodePresenter
    {
        protected override IEnumerable<GraphElementPresenter> GetControlData()
        {
            var instance = CreateInstance<GradientContolPresenter>();
            instance.Initialize(node);
            return new List<GraphElementPresenter> { instance };
        }
    }

    [Serializable]
    public class GradientObject : ScriptableObject
    {
        public Gradient gradient = new Gradient();
    }
}
