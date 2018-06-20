using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.Rendering
{
    class SerializedShadowInitParameters
    {
        public SerializedProperty root;

        public SerializedProperty shadowAtlasWidth;
        public SerializedProperty shadowAtlasHeight;
//forest-begin: 16-bit shadows option
        public SerializedProperty shadowMap16Bit;
//forest-end:

        public SerializedShadowInitParameters(SerializedProperty root)
        {
            this.root = root;

            shadowAtlasWidth = root.Find((ShadowInitParameters s) => s.shadowAtlasWidth);
            shadowAtlasHeight = root.Find((ShadowInitParameters s) => s.shadowAtlasHeight);
//forest-begin: 16-bit shadows option
			shadowMap16Bit = root.Find((ShadowInitParameters s) => s.shadowMap16Bit);
//forest-end:
        }
    }
}
