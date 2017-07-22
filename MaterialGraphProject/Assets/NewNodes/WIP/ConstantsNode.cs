﻿using System.Collections.Generic;
using UnityEngine.MaterialGraph;
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    [Title("Math/Constants")]
    public class ConstantsNode : PropertyNode, IGeneratesBodyCode
    {

        static Dictionary<ConstantType, float> m_constantList = new Dictionary<ConstantType, float>
        {
            {ConstantType.PI, 3.1415926f },
            {ConstantType.TAU, 6.28318530f},
            {ConstantType.PHI, 1.618034f},
            {ConstantType.E, 2.718282f},
            {ConstantType.SQRT2, 1.414214f},
        };

        [SerializeField]
        private ConstantType m_constant = ConstantType.PI;

        private const int kOutputSlotId = 0;
        private const string kOutputSlotName = "Constant";

        public ConstantType constant
        {
            get { return m_constant; }
            set
            {
                if (m_constant == value)
                    return;

                m_constant = value;
                if (onModified != null)
                {
                    onModified(this, ModificationScope.Graph);
                }
            }
        }

        public ConstantsNode()
        {
            name = "MathConstant";
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new MaterialSlot(kOutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, SlotValueType.Vector1, Vector4.zero));
            RemoveSlotsNameNotMatching(new[] { kOutputSlotId });
        }

        public override PropertyType propertyType
        {
            get { return PropertyType.Float; }
        }

        public override PreviewProperty GetPreviewProperty()
        {
            return new PreviewProperty
            {
                m_Name = propertyName,
                m_PropType = PropertyType.Float,
                m_Float = m_constantList[constant]
            };
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            visitor.AddShaderChunk(precision + " " + propertyName + " = " + m_constantList[constant] + ";", true);
        }
    }
}
