using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    [Title("Input/Vector/Vector 2")]
    public class Vector2Node : PropertyNode, IGeneratesBodyCode
    {
        private const int kOutputSlotId = 0;
        private const string kOutputSlotName = "Value";

        [SerializeField]
        private Vector2 m_Value;

        public Vector2Node()
        {
            name = "Vector2";
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new MaterialSlot(kOutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, SlotValueType.Vector2, Vector4.zero));
            RemoveSlotsNameNotMatching(new[] { kOutputSlotId });
        }

        public override PropertyType propertyType
        {
            get { return PropertyType.Vector2; }
        }

        public Vector2 value
        {
            get { return m_Value; }
            set
            {
                if (m_Value == value)
                    return;

                m_Value = value;

                if (onModified != null)
                    onModified(this, ModificationScope.Node);
            }
        }

        public override void GeneratePropertyBlock(PropertyGenerator visitor, GenerationMode generationMode)
        {
            if (exposedState == ExposedState.Exposed)
                visitor.AddShaderProperty(new VectorPropertyChunk(propertyName, description, m_Value, PropertyChunk.HideState.Visible));
        }

        public override void GeneratePropertyUsages(ShaderGenerator visitor, GenerationMode generationMode)
        {
            if (exposedState == ExposedState.Exposed || generationMode.IsPreview())
                visitor.AddShaderChunk(precision + "2 " + propertyName + ";", true);
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            if (exposedState == ExposedState.Exposed || generationMode.IsPreview())
                return;

            visitor.AddShaderChunk(precision + "2 " + propertyName + " = " + precision + "2 (" + m_Value.x + ", " + m_Value.y + ");", true);
        }

        public override PreviewProperty GetPreviewProperty()
        {
            return new PreviewProperty
                   {
                       m_Name = propertyName,
                       m_PropType = PropertyType.Vector2,
                       m_Vector4 = m_Value
                   };
        }
    }
}
