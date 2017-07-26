using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing.Drawing;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEngine.Graphing;
using UnityEngine.MaterialGraph;

namespace UnityEditor.MaterialGraph.Drawing
{
    public abstract class AbstractMaterialGraphInspector : AbstractGraphInspector
    {
        private bool m_RequiresTime;

        protected GUIContent m_Title = new GUIContent();

        private NodePreviewPresenter m_NodePreviewPresenter;

        private AbstractMaterialNode m_PreviewNode;

        protected AbstractMaterialNode previewNode
        {
            get { return m_PreviewNode; }
            set
            {
                if (value == m_PreviewNode)
                    return;
                // ReSharper disable once DelegateSubtraction
                // This is only an issue when subtracting a list of callbacks
                // (which will subtract that specific subset, i.e. `{A B C} - {A B} = {C}`, but `{A B C} - {A C} -> {A B C}`)
                ForEachChild(m_PreviewNode, (node) => node.onModified -= OnPreviewNodeModified);
                m_PreviewNode = value;
                m_NodePreviewPresenter.Initialize(value);
                m_Title.text = m_PreviewNode.name;
                m_RequiresTime = false;
                ForEachChild(m_PreviewNode,
                    (node) =>
                    {
                        node.onModified += OnPreviewNodeModified;
                        m_RequiresTime |= node is IRequiresTime;
                    });
            }
        }

        protected AbstractMaterialGraphInspector() : base(typeMappings)
        {
        }

        private static IEnumerable<TypeMapping> typeMappings
        {
            get
            {
				yield return new TypeMapping(typeof(AbstractSurfaceMasterNode), typeof(SurfaceMasterNodeInspector));
				yield return new TypeMapping(typeof(PropertyNode), typeof(PropertyNodeInspector));
                yield return new TypeMapping(typeof(SubGraphInputNode), typeof(SubgraphInputNodeInspector));
                yield return new TypeMapping(typeof(SubGraphOutputNode), typeof(SubgraphOutputNodeInspector));
            }
        }

        private void OnPreviewNodeModified(INode node, ModificationScope scope)
        {
            m_NodePreviewPresenter.modificationScope = scope;
            Repaint();
        }

        public override void OnEnable()
        {
            m_NodePreviewPresenter = CreateInstance<NodePreviewPresenter>();
            base.OnEnable();
            previewNode = null;
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (m_PreviewNode != null)
                {
                    var size = Mathf.Min(r.width, r.height);
                    var image = m_NodePreviewPresenter.Render(new Vector2(size, size));
                    GUI.DrawTexture(r, image, ScaleMode.ScaleToFit);
                   // m_NodePreviewPresenter.modificationScope = ModificationScope.Node;
                }
                else
                {
                    EditorGUI.DropShadowLabel(r, "No node pinned");
                }
            }
        }

        public override void OnPreviewSettings()
        {
            GUI.enabled = selectedNodes.Count() <= 1 && selectedNodes.FirstOrDefault() != previewNode;
            if (GUILayout.Button("Pin selected", "preButton"))
                previewNode = selectedNodes.FirstOrDefault() as AbstractMaterialNode;
            GUI.enabled = true;
        }

        public override bool RequiresConstantRepaint()
        {
            return m_RequiresTime;
        }

        public override GUIContent GetPreviewTitle()
        {
            return m_Title;
        }

        private void ForEachChild(INode node, Action<INode> action)
        {
            if (node == null)
                return;
            using (var childNodes = ListPool<INode>.GetDisposable())
            {
                NodeUtils.DepthFirstCollectNodesFromNode(childNodes.value, node);
                foreach (var childNode in childNodes.value)
                {
                    action(childNode);
                }
            }
        }
    }
}
