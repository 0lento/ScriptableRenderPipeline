using System.Reflection;

namespace UnityEngine.MaterialGraph
{
    [Title("Math/Vector/TangentToWorld")]
    public class TangentToWorldNode : CodeFunctionNode
    {
        public TangentToWorldNode()
        {
            name = "TangentToWorld";
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("Unity_TangentToWorld", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_TangentToWorld(
            [Slot(0, Binding.None)] Vector3 inVector,
            [Slot(1, Binding.None)] out Vector3 result,
            [Slot(2, Binding.Tangent)] Vector3 tangent,
            [Slot(3, Binding.Bitangent)] Vector3 biTangent,
            [Slot(4, Binding.Normal)] Vector3 normal)
        {
            result = Vector3.zero;
            return
                @"
{
    {precision}3x3 tangentToWorld = transpose({precision}3x3(tangent, biTangent, normal));
    result= saturate(mul(tangentToWorld, normalize(inVector)));
}
";
        }
    }
}
