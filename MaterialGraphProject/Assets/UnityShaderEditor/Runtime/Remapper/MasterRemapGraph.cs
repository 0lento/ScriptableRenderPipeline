using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    [Serializable]
    public class MasterRemapGraph : AbstractSubGraph
    {

        public override void AddNode(INode node)
        {
            var materialNode = node as AbstractMaterialNode;
            if (materialNode != null)
            {
                var amn = materialNode;
                if (!amn.allowedInRemapGraph)
                {
                    Debug.LogWarningFormat("Attempting to add {0} to Sub Graph. This is not allowed.", amn.GetType());
                    return;
                }
            }
            base.AddNode(node);
        }

        public override IEnumerable<AbstractMaterialNode> activeNodes
        {
            get
            {
                var masters = GetNodes<MasterNode>();
                var referencedNodes = new List<INode>();
                foreach (var master in masters)
                    NodeUtils.DepthFirstCollectNodesFromNode(referencedNodes, master);

                return referencedNodes.OfType<AbstractMaterialNode>();
            }
        }
    }
}
