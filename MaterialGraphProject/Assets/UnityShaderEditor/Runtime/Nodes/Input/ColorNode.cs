using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    [Title("Input/Color")]
    public class ColorNode : PropertyNode, IGeneratesBodyCode
    {
		[SerializeField]
		private bool m_HDR;

		[SerializeField]
        private Color m_Color;

        private const int kOutputSlotId = 0;
        private const string kOutputSlotName = "Color";

		public bool HDR
		{
			get { return m_HDR; }
			set
			{
				if (m_HDR == value)
					return;

				m_HDR = value;
				if (onModified != null)
				{
					onModified(this, ModificationScope.Node);
				}
			}
		}

        public ColorNode()
        {
            name = "Color";
            UpdateNodeAfterDeserialization();
        }

        public override bool hasPreview
        {
            get { return true; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new MaterialSlot(kOutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, SlotValueType.Vector4, Vector4.zero));
            RemoveSlotsNameNotMatching(new[] { kOutputSlotId });
        }

        public override PropertyType propertyType
        {
            get { return PropertyType.Color; }
        }

        public Color color
        {
            get { return m_Color; }
            set
            {
                if (m_Color == value)
                    return;

                m_Color = value;
                if (onModified != null)
                {
                    onModified(this, ModificationScope.Node);
                }
            }
        }

        public override void GeneratePropertyBlock(PropertyGenerator visitor, GenerationMode generationMode)
        {
            if (exposedState == ExposedState.Exposed)
				visitor.AddShaderProperty(new ColorPropertyChunk(propertyName, description, color, m_HDR ? ColorPropertyChunk.ColorType.HDR : ColorPropertyChunk.ColorType.Default , PropertyChunk.HideState.Visible));
        }

        public override void GeneratePropertyUsages(ShaderGenerator visitor, GenerationMode generationMode)
        {
            if (exposedState == ExposedState.Exposed || generationMode.IsPreview())
                visitor.AddShaderChunk(precision + "4 " + propertyName + ";", true);
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            // we only need to generate node code if we are using a constant... otherwise we can just refer to the property :)
            if (exposedState == ExposedState.Exposed || generationMode.IsPreview())
                return;

            visitor.AddShaderChunk(precision + "4 " + propertyName + " = " + precision + "4 (" + color.r + ", " + color.g + ", " + color.b + ", " + color.a + ");", true);
        }

        public override PreviewProperty GetPreviewProperty()
        {
            return new PreviewProperty
                   {
                       m_Name = propertyName,
                       m_PropType = PropertyType.Color,
                       m_Color = color
                   };
        }
    }
}
