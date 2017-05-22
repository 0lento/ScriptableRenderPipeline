using System;
using System.Collections.Generic;
using RMGUI.GraphView;
using UnityEditor.Graphing.Drawing;
using UnityEngine.MaterialGraph;

namespace UnityEditor.MaterialGraph.Drawing
{
    class SwizzleControlPresenter : GraphControlPresenter
    {
        public override void OnGUIHandler()
        {
            base.OnGUIHandler();

            var tNode = node as SwizzleNode;
            if (tNode == null)
                return;

            SwizzleNode.SwizzleChannel[] newSwizzleChannels = (SwizzleNode.SwizzleChannel[])tNode.swizzleChannels.Clone();
            EditorGUI.BeginChangeCheck();
            newSwizzleChannels[0] = (SwizzleNode.SwizzleChannel)EditorGUILayout.EnumPopup("Red output:", newSwizzleChannels[0]);
            newSwizzleChannels[1] = (SwizzleNode.SwizzleChannel)EditorGUILayout.EnumPopup("Green output:", newSwizzleChannels[1]);
            newSwizzleChannels[2] = (SwizzleNode.SwizzleChannel)EditorGUILayout.EnumPopup("Blue output:", newSwizzleChannels[2]);
            newSwizzleChannels[3] = (SwizzleNode.SwizzleChannel)EditorGUILayout.EnumPopup("Alpha output:", newSwizzleChannels[3]);
            if (EditorGUI.EndChangeCheck())
            {
                tNode.swizzleChannels = newSwizzleChannels;
            }
        }

        public override float GetHeight()
        {
            return (EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing) + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    [Serializable]
    public class SwizzleNodePresenter : PropertyNodePresenter
    {
        protected override IEnumerable<GraphElementPresenter> GetControlData()
        {
            var instance = CreateInstance<SwizzleControlPresenter>();
            instance.Initialize(node);
            return new List<GraphElementPresenter>(base.GetControlData()) { instance };
        }
    }
}
