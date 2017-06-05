﻿using System.Reflection;

namespace UnityEngine.MaterialGraph
{
    [Title("Math/Trigonometry/Cos")]
    public class CosNode : CodeFunctionNode
    {
        public CosNode()
        {
            name = "Cos";
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("Unity_Cos", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_Cos(
            [Slot(0, Binding.None)] DynamicDimensionVector argument,
            [Slot(1, Binding.None)] out DynamicDimensionVector result)
        {
            return
                @"
{
    result = cos(argument);
}
";
        }
    }
}
