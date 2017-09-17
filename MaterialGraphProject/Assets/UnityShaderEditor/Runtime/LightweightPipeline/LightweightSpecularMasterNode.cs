using System;
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    [Serializable]
    [Title("Master/Lightweight/PBR Specular")]
    public class LightweightSpecularMasterNode : AbstractLightweightPBRMasterNode
    {
        public const string SpecularSlotName = "Specular";
        public const int SpecularSlotId = 2;

        public const string WorkflowName = "Specular";

        public LightweightSpecularMasterNode()
        {
            name = "LightweightSpecularMasterNode";
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new MaterialSlot(VertexOffsetId, VertexOffsetName, VertexOffsetName, SlotType.Input, SlotValueType.Vector3, Vector4.zero, ShaderStage.Vertex));
            AddSlot(new MaterialSlot(AlbedoSlotId, AlbedoSlotName, AlbedoSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero, ShaderStage.Fragment));
            AddSlot(new MaterialSlot(NormalSlotId, NormalSlotName, NormalSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero, ShaderStage.Fragment));
            AddSlot(new MaterialSlot(EmissionSlotId, EmissionSlotName, EmissionSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero, ShaderStage.Fragment));
            AddSlot(new MaterialSlot(SpecularSlotId, SpecularSlotName, SpecularSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero, ShaderStage.Fragment));
            AddSlot(new MaterialSlot(SmoothnessSlotId, SmoothnessSlotName, SmoothnessSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero, ShaderStage.Fragment));
            AddSlot(new MaterialSlot(OcclusionSlotId, OcclusionSlotName, OcclusionSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero, ShaderStage.Fragment));
            AddSlot(new MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero, ShaderStage.Fragment));

            // clear out slot names that do not match the slots
            // we support
            RemoveSlotsNameNotMatching(
                new[]
            {
                AlbedoSlotId,
                NormalSlotId,
                EmissionSlotId,
                SpecularSlotId,
                SmoothnessSlotId,
                OcclusionSlotId,
                AlphaSlotId,
                VertexOffsetId
            });
        }

        protected override int[] surfaceInputs
        {
            get
            {
                return new[]
                {
                    AlbedoSlotId,
                    NormalSlotId,
                    EmissionSlotId,
                    SpecularSlotId,
                    SmoothnessSlotId,
                    OcclusionSlotId,
                    AlphaSlotId,
                };
            }
        }

        protected override int[] vertexInputs
        {
            get
            {
                return new[]
                {
                    VertexOffsetId
                };
            }
        }

        public override string GetWorkflowName()
        {
            return WorkflowName;
        }
    }
}
