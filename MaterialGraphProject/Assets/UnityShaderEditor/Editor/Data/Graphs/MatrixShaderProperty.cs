using System;
using System.Text;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public abstract class MatrixShaderProperty : AbstractShaderProperty<Matrix4x4>
    {
        public override string GetPropertyBlockString()
        {
            return string.Empty;
        }

        public override Vector4 defaultValue
        {
            get { return new Vector4(); }
        }

        public override string GetPropertyDeclarationString(string delimiter = ";")
        {
            return "float4x4 " + referenceName + ";";
        }

        public override PreviewProperty GetPreviewMaterialProperty()
        {
            return default(PreviewProperty);
        }        
    }
}
