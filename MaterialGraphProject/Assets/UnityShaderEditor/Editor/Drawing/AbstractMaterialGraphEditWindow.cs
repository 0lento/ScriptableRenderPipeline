using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Graphing;
using UnityEngine.MaterialGraph;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UnityEditor.MaterialGraph.Drawing
{
    public interface IMaterialGraphEditWindow
    {
        void PingAsset();

        void UpdateAsset();

        void Repaint();

        void ToggleRequiresTime();
        void ToSubGraph();

        void Show();
        void Focus();
        Object selected { get; set; }
        void ChangeSelection(Object newSelection);
    }

    public abstract class HelperMaterialGraphEditWindow : EditorWindow, IMaterialGraphEditWindow
    {
        public abstract AbstractMaterialGraph GetMaterialGraph();
        public abstract void PingAsset();
        public abstract void UpdateAsset();
        public abstract void ToggleRequiresTime();
        public abstract void ToSubGraph();
        public abstract Object selected { get; set; }
        public abstract void ChangeSelection(Object newSelection);
    }

    public class MaterialGraphEditWindow : AbstractMaterialGraphEditWindow<UnityEngine.MaterialGraph.MaterialGraph>
    {
        public override AbstractMaterialGraph GetMaterialGraph()
        {
            return inMemoryAsset;
        }
    }

    public class SubGraphEditWindow : AbstractMaterialGraphEditWindow<SubGraph>
    {
        public override AbstractMaterialGraph GetMaterialGraph()
        {
            return inMemoryAsset;
        }
    }

    public class MasterReampGraphEditWindow : AbstractMaterialGraphEditWindow<MasterRemapGraph>
    {
        public override AbstractMaterialGraph GetMaterialGraph()
        {
            return inMemoryAsset;
        }
    }

    public abstract class AbstractMaterialGraphEditWindow<TGraphType> : HelperMaterialGraphEditWindow where TGraphType : AbstractMaterialGraph
    {
        public static bool allowAlwaysRepaint = true;

        [SerializeField]
        Object m_Selected;

        [SerializeField]
        TGraphType m_InMemoryAsset;

        GraphEditorView m_GraphEditorView;
        GraphEditorView graphEditorView
        {
            get { return m_GraphEditorView; }
            set
            {
                if (m_GraphEditorView != null)
                {
                    if (m_GraphEditorView.presenter != null)
                        m_GraphEditorView.presenter.Dispose();
                    m_GraphEditorView.Dispose();
                }
                m_GraphEditorView = value;
            }
        }

        protected TGraphType inMemoryAsset
        {
            get { return m_InMemoryAsset; }
            set { m_InMemoryAsset = value; }
        }

        public override Object selected
        {
            get { return m_Selected; }
            set { m_Selected = value; }
        }

        void Update()
        {
            if (graphEditorView != null)
            {
                if (graphEditorView.presenter == null)
                    CreatePresenter();
                if (graphEditorView.presenter != null)
                    graphEditorView.presenter.UpdatePreviews();
            }
        }

        void OnEnable()
        {
            graphEditorView = new GraphEditorView();
            rootVisualContainer.Add(graphEditorView);
        }

        void OnDisable()
        {
            rootVisualContainer.Clear();
            graphEditorView = null;
        }

        void OnDestroy()
        {
            if (EditorUtility.DisplayDialog("Shader Graph Might Have Been Modified", "Do you want to save the changes you made in the shader graph?", "Save", "Don't Save"))
            {
                UpdateAsset();
            }
            graphEditorView = null;
        }

        void OnGUI()
        {
            var presenter = graphEditorView.presenter;
            var e = Event.current;

            if (e.type == EventType.ValidateCommand && (
                    e.commandName == "Copy" && presenter.graphPresenter.canCopy
                    || e.commandName == "Paste" && presenter.graphPresenter.canPaste
                    || e.commandName == "Duplicate" && presenter.graphPresenter.canDuplicate
                    || e.commandName == "Cut" && presenter.graphPresenter.canCut
                    || (e.commandName == "Delete" || e.commandName == "SoftDelete") && presenter.graphPresenter.canDelete))
            {
                e.Use();
            }

            if (e.type == EventType.ExecuteCommand)
            {
                if (e.commandName == "Copy")
                    presenter.graphPresenter.Copy();
                if (e.commandName == "Paste")
                    presenter.graphPresenter.Paste();
                if (e.commandName == "Duplicate")
                    presenter.graphPresenter.Duplicate();
                if (e.commandName == "Cut")
                    presenter.graphPresenter.Cut();
                if (e.commandName == "Delete" || e.commandName == "SoftDelete")
                    presenter.graphPresenter.Delete();
            }

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.A)
                    graphEditorView.graphView.FrameAll();
                if (e.keyCode == KeyCode.F)
                    graphEditorView.graphView.FrameSelection();
                if (e.keyCode == KeyCode.O)
                    graphEditorView.graphView.FrameOrigin();
                if (e.keyCode == KeyCode.Tab)
                    graphEditorView.graphView.FrameNext();
                if (e.keyCode == KeyCode.Tab && e.modifiers == EventModifiers.Shift)
                    graphEditorView.graphView.FramePrev();
            }
        }

        public override void PingAsset()
        {
            if (selected != null)
                EditorGUIUtility.PingObject(selected);
        }

        public override void UpdateAsset()
        {
            if (selected != null && inMemoryAsset != null)
            {
                var path = AssetDatabase.GetAssetPath(selected);
                if (string.IsNullOrEmpty(path) || inMemoryAsset == null)
                {
                    return;
                }

                if (typeof(TGraphType) == typeof(UnityEngine.MaterialGraph.MaterialGraph))
                    UpdateShaderGraphOnDisk(path);

                if (typeof(TGraphType) == typeof(SubGraph))
                    UpdateAbstractSubgraphOnDisk<SubGraph>(path);

                if (typeof(TGraphType) == typeof(MasterRemapGraph))
                    UpdateAbstractSubgraphOnDisk<MasterRemapGraph>(path);
            }
        }

        public override void ToSubGraph()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save subgraph", "New SubGraph", "ShaderSubGraph", "");
            path = path.Replace(Application.dataPath, "Assets");
            if (path.Length == 0)
                return;

            var graphPresenter = graphEditorView.presenter.graphPresenter;
            var selected = graphPresenter.elements.Where(e => e.selected);

            var filtered = new List<GraphElementPresenter>();

            foreach (var presenter in selected)
            {
                var nodePresenter = presenter as MaterialNodePresenter;
                if (nodePresenter != null)
                {
                    if (!(nodePresenter.node is PropertyNode))
                        filtered.Add(nodePresenter);
                }
                else
                {
                    filtered.Add(presenter);
                }
            }

            var deserialized = MaterialGraphPresenter.DeserializeCopyBuffer(JsonUtility.ToJson(MaterialGraphPresenter.CreateCopyPasteGraph(filtered)));

            if (deserialized == null)
                return;

            var graph = new SubGraph();
            graph.AddNode(new SubGraphOutputNode());

            var nodeGuidMap = new Dictionary<Guid, Guid>();
            foreach (var node in deserialized.GetNodes<INode>())
            {
                var oldGuid = node.guid;
                var newGuid = node.RewriteGuid();
                nodeGuidMap[oldGuid] = newGuid;
                graph.AddNode(node);
            }

            // remap outputs to the subgraph
            var onlyInputInternallyConnected = new List<IEdge>();
            var onlyOutputInternallyConnected = new List<IEdge>();
            foreach (var edge in deserialized.edges)
            {
                var outputSlot = edge.outputSlot;
                var inputSlot = edge.inputSlot;

                Guid remappedOutputNodeGuid;
                Guid remappedInputNodeGuid;
                var outputRemapExists = nodeGuidMap.TryGetValue(outputSlot.nodeGuid, out remappedOutputNodeGuid);
                var inputRemapExists = nodeGuidMap.TryGetValue(inputSlot.nodeGuid, out remappedInputNodeGuid);

                // pasting nice internal links!
                if (outputRemapExists && inputRemapExists)
                {
                    var outputSlotRef = new SlotReference(remappedOutputNodeGuid, outputSlot.slotId);
                    var inputSlotRef = new SlotReference(remappedInputNodeGuid, inputSlot.slotId);
                    graph.Connect(outputSlotRef, inputSlotRef);
                }
                // one edge needs to go to outside world
                else if (outputRemapExists)
                {
                    onlyOutputInternallyConnected.Add(edge);
                }
                else if (inputRemapExists)
                {
                    onlyInputInternallyConnected.Add(edge);
                }
            }

            var uniqueInputEdges = onlyOutputInternallyConnected.GroupBy(
                edge => edge.outputSlot,
                edge => edge,
                (key, edges) => new {slotRef = key, edges = edges.ToList()});
            var inputsNeedingConnection = new List<KeyValuePair<IEdge, IEdge>>();
            foreach (var group in uniqueInputEdges)
            {
                var inputNode = new PropertyNode();

                var sr = group.slotRef;
                var fromNode = graphPresenter.graph.GetNodeFromGuid(sr.nodeGuid);
                var fromSlot = fromNode.FindOutputSlot<MaterialSlot>(sr.slotId);

                switch (fromSlot.concreteValueType)
                {
                    case ConcreteSlotValueType.SamplerState:
                        break;
                    case ConcreteSlotValueType.Matrix4:
                        break;
                    case ConcreteSlotValueType.Matrix3:
                        break;
                    case ConcreteSlotValueType.Matrix2:
                        break;
                    case ConcreteSlotValueType.Texture2D:
                        break;
                    case ConcreteSlotValueType.Vector4:
                        break;
                    case ConcreteSlotValueType.Vector3:
                        break;
                    case ConcreteSlotValueType.Vector2:
                        break;
                    case ConcreteSlotValueType.Vector1:
                        break;
                    case ConcreteSlotValueType.Error:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var uniqueOutputEdges = onlyInputInternallyConnected.GroupBy(
                edge => edge.inputSlot,
                edge => edge,
                (key, edges) => new {slot = key, edges = edges.ToList()});

            var outputsNeedingConnection = new List<KeyValuePair<IEdge, IEdge>>();
            foreach (var group in uniqueOutputEdges)
            {
                var outputNode = graph.outputNode;
                var slotId = outputNode.AddSlot();

                var inputSlotRef = new SlotReference(outputNode.guid, slotId);

                foreach (var edge in group.edges)
                {
                    var newEdge = graph.Connect(new SlotReference(nodeGuidMap[edge.outputSlot.nodeGuid], edge.outputSlot.slotId), inputSlotRef);
                    outputsNeedingConnection.Add(new KeyValuePair<IEdge, IEdge>(edge, newEdge));
                }
            }

            File.WriteAllText(path, EditorJsonUtility.ToJson(graph));
            AssetDatabase.ImportAsset(path);

            var subGraph = AssetDatabase.LoadAssetAtPath(path, typeof(MaterialSubGraphAsset)) as MaterialSubGraphAsset;
            if (subGraph == null)
                return;

            var subGraphNode = new SubGraphNode();
            graphPresenter.AddNode(subGraphNode);
            subGraphNode.subGraphAsset = subGraph;

          /*  foreach (var edgeMap in inputsNeedingConnection)
            {
                graphPresenter.graph.Connect(edgeMap.Key.outputSlot, new SlotReference(subGraphNode.guid, edgeMap.Value.outputSlot.slotId));
            }*/

            foreach (var edgeMap in outputsNeedingConnection)
            {
                graphPresenter.graph.Connect(new SlotReference(subGraphNode.guid, edgeMap.Value.inputSlot.slotId), edgeMap.Key.inputSlot);
            }

            var toDelete = graphPresenter.elements.Where(e => e.selected).OfType<MaterialNodePresenter>();
            graphPresenter.RemoveElements(toDelete, new List<GraphEdgePresenter>());
        }

        private void UpdateAbstractSubgraphOnDisk<T>(string path) where T : AbstractSubGraph
        {
            var graph = inMemoryAsset as T;
            if (graph == null)
                return;

            File.WriteAllText(path, EditorJsonUtility.ToJson(inMemoryAsset, true));
            AssetDatabase.ImportAsset(path);
        }


        private void UpdateShaderGraphOnDisk(string path)
        {
            var graph = inMemoryAsset as UnityEngine.MaterialGraph.MaterialGraph;
            if (graph == null)
                return;

            List<PropertyCollector.TextureInfo> configuredTextures;
            graph.GetShader(Path.GetFileNameWithoutExtension(path), GenerationMode.ForReals, out configuredTextures);

            var shaderImporter = AssetImporter.GetAtPath(path) as ShaderImporter;
            if (shaderImporter == null)
                return;

            var textureNames = new List<string>();
            var textures = new List<Texture>();
            foreach (var textureInfo in configuredTextures.Where(x => x.modifiable))
            {
                var texture = EditorUtility.InstanceIDToObject(textureInfo.textureId) as Texture;
                if (texture == null)
                    continue;
                textureNames.Add(textureInfo.name);
                textures.Add(texture);
            }
            shaderImporter.SetDefaultTextures(textureNames.ToArray(), textures.ToArray());

            textureNames.Clear();
            textures.Clear();
            foreach (var textureInfo in configuredTextures.Where(x => !x.modifiable))
            {
                var texture = EditorUtility.InstanceIDToObject(textureInfo.textureId) as Texture;
                if (texture == null)
                    continue;
                textureNames.Add(textureInfo.name);
                textures.Add(texture);
            }
            shaderImporter.SetNonModifiableTextures(textureNames.ToArray(), textures.ToArray());
            File.WriteAllText(path, EditorJsonUtility.ToJson(inMemoryAsset, true));
            shaderImporter.SaveAndReimport();
            AssetDatabase.ImportAsset(path);
        }

        public override void ToggleRequiresTime()
        {
            allowAlwaysRepaint = !allowAlwaysRepaint;
        }

        public override void ChangeSelection(Object newSelection)
        {
            if (!EditorUtility.IsPersistent(newSelection))
                return;

            if (selected == newSelection)
                return;

            selected = newSelection;

            var path = AssetDatabase.GetAssetPath(newSelection);
            var textGraph = File.ReadAllText(path, Encoding.UTF8);
            inMemoryAsset = JsonUtility.FromJson<TGraphType>(textGraph);
            inMemoryAsset.OnEnable();
            inMemoryAsset.ValidateGraph();

            CreatePresenter();
            titleContent = new GUIContent(selected.name);

            Repaint();
        }

        void CreatePresenter()
        {
            if (graphEditorView.presenter != null)
                graphEditorView.presenter.Dispose();
            var presenter = CreateInstance<GraphEditorPresenter>();
            presenter.Initialize(inMemoryAsset, this, selected.name);
            graphEditorView.presenter = presenter;
            graphEditorView.RegisterCallback<PostLayoutEvent>(OnPostLayout);
        }

        void OnPostLayout(PostLayoutEvent evt)
        {
            graphEditorView.UnregisterCallback<PostLayoutEvent>(OnPostLayout);
            graphEditorView.graphView.FrameAll();
        }
    }
}
