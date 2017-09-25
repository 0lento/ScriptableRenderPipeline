using System;

namespace UnityEngine.MaterialGraph
{
    [Serializable]
    public class Vector4ShaderProperty : VectorShaderProperty
    {
        public override PropertyType propertyType
        {
            get { return PropertyType.Vector4; }
        }

        public override Vector4 defaultValue
        {
            get { return value; }
        }

        public override PreviewProperty GetPreviewMaterialProperty()
        {
            return new PreviewProperty()
            {
                m_Name = name,
                m_PropType = PropertyType.Vector4,
                m_Vector4 = value
            };
        }
    }
}
