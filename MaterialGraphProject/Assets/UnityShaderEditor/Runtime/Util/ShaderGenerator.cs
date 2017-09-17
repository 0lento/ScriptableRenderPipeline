using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    public static class ShaderGeneratorNames
    {
        private static string[] UV = {"uv0", "uv1", "uv2", "uv3"};
        public static int UVCount = 4;

        public const string ObjectSpaceNormal = "objectSpaceNormal";
        public const string ViewSpaceNormal = "viewSpaceNormal";
        public const string WorldSpaceNormal = "worldSpaceNormal";
        public const string TangentSpaceNormal = "tangentSpaceNormal";

        public const string ObjectSpaceBiTangent = "objectSpaceBiTangent";
        public const string ViewSpaceBiTangent = "viewSpaceBiTangent";
        public const string WorldSpaceSpaceBiTangent = "worldSpaceSpaceBiTangent";
        public const string TangentSpaceBiTangent = "TangentSpaceBitangent";

        public const string ObjectSpaceTangent = "objectSpaceTangent";
        public const string ViewSpaceTangent = "viewSpaceTangent";
        public const string WorldSpaceTangent = "worldSpaceSpaceTangent";
        public const string TangentSpaceTangent = "tangentSpaceSpaceTangent";

        public const string ObjectSpaceViewDirection = "objectSpaceViewDirection";
        public const string ViewSpaceViewDirection = "viewSpaceViewDirection";
        public const string WorldSpaceViewDirection = "worldSpaceViewDirection";
        public const string TangentSpaceViewDirection = "tangentSpaceViewDirection";

        public const string ObjectSpacePosition = "objectSpacePosition";
        public const string ViewSpacePosition = "viewSpaceVPosition";
        public const string WorldSpacePosition = "worldSpacePosition";
        public const string TangentSpacePosition = "tangentSpacePosition";

        public const string ScreenPosition = "screenPosition";
        public const string VertexColor = "vertexColor";


        public static string GetUVName(this UVChannel channel)
        {
            return UV[(int)channel];
        }
    }

    public enum UVChannel
    {
        uv0 = 0,
        uv1 = 1,
        uv2 = 2,
        uv3 = 3,
    }

    public class ShaderGenerator
    {
        private struct ShaderChunk
        {
            public ShaderChunk(int indentLevel, string shaderChunkString)
            {
                m_IndentLevel = indentLevel;
                m_ShaderChunkString = shaderChunkString;
            }

            private readonly int m_IndentLevel;
            private readonly string m_ShaderChunkString;

            public int chunkIndentLevel
            {
                get { return m_IndentLevel; }
            }

            public string chunkString
            {
                get { return m_ShaderChunkString; }
            }
        }

        private readonly List<ShaderChunk> m_ShaderChunks = new List<ShaderChunk>();
        private int m_IndentLevel;
        private string m_Pragma = string.Empty;

        public void AddPragmaChunk(string s)
        {
            m_Pragma += s;
        }

        public string GetPragmaString()
        {
            return m_Pragma;
        }

        public void AddShaderChunk(string s, bool unique)
        {
            if (string.IsNullOrEmpty(s))
                return;

            if (unique && m_ShaderChunks.Any(x => x.chunkString == s))
                return;

            m_ShaderChunks.Add(new ShaderChunk(m_IndentLevel, s));
        }

        public void Indent()
        {
            m_IndentLevel++;
        }

        public void Deindent()
        {
            m_IndentLevel = Math.Max(0, m_IndentLevel - 1);
        }

        public string GetShaderString(int baseIndentLevel)
        {
            var sb = new StringBuilder();
            foreach (var shaderChunk in m_ShaderChunks)
            {
                var lines = shaderChunk.chunkString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    for (var i = 0; i < shaderChunk.chunkIndentLevel + baseIndentLevel; i++)
                        sb.Append("\t");

                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }

        internal static string GetTemplatePath(string templateName)
        {
            var path = new List<string>
            {
                Application.dataPath,
                "UnityShaderEditor",
                "Editor",
                "Templates"
            };

            string result = path[0];
            for (int i = 1; i < path.Count; i++)
                result = Path.Combine(result, path[i]);

            result = Path.Combine(result, templateName);
            return result;
        }

        private const string kErrorString = @"ERROR!";

        public static string AdaptNodeOutput(AbstractMaterialNode node, int outputSlotId, ConcreteSlotValueType convertToType, bool textureSampleUVHack = false)
        {
            var outputSlot = node.FindOutputSlot<MaterialSlot>(outputSlotId);

            if (outputSlot == null)
                return kErrorString;

            var convertFromType = outputSlot.concreteValueType;
            var rawOutput = node.GetVariableNameForSlot(outputSlotId);
            if (convertFromType == convertToType)
                return rawOutput;

            switch (convertToType)
            {
                case ConcreteSlotValueType.Vector1:
                    return string.Format("({0}).x", rawOutput);
                case ConcreteSlotValueType.Vector2:
                    switch (convertFromType)
                    {
                        case ConcreteSlotValueType.Vector1:
                            return string.Format("({0}{1})", rawOutput, ".xx");
                        case ConcreteSlotValueType.Vector3:
                        case ConcreteSlotValueType.Vector4:
                            return string.Format("({0}.xy)", rawOutput);
                        default:
                            return kErrorString;
                    }
                case ConcreteSlotValueType.Vector3:
                    switch (convertFromType)
                    {
                        case ConcreteSlotValueType.Vector1:
                            return string.Format("({0}{1})", rawOutput, ".xxx");
                        case ConcreteSlotValueType.Vector4:
                            return string.Format("({0}.xyz)", rawOutput);
                        default:
                            return kErrorString;
                    }
                case ConcreteSlotValueType.Vector4:
                    switch (convertFromType)
                    {
                        case ConcreteSlotValueType.Vector1:
                            return string.Format("({0}{1})", rawOutput, ".xxxx");
                        default:
                            return kErrorString;
                    }
                default:
                    return kErrorString;
            }
        }

        public static string AdaptNodeOutputForPreview(AbstractMaterialNode node, int outputSlotId)
        {
            var outputSlot = node.FindOutputSlot<MaterialSlot>(outputSlotId);

            if (outputSlot == null)
                return kErrorString;

            var convertFromType = outputSlot.concreteValueType;

            var rawOutput = node.GetVariableNameForSlot(outputSlotId);

            // preview is always dimension 4, and we always ignore alpha
            switch (convertFromType)
            {
                case ConcreteSlotValueType.Vector1:
                    return string.Format("half4({0}, {0}, {0}, 1.0)", rawOutput);
                case ConcreteSlotValueType.Vector2:
                    return string.Format("half4({0}.x, {0}.y, 0.0, 1.0)", rawOutput);
                case ConcreteSlotValueType.Vector3:
                    return string.Format("half4({0}.x, {0}.y, {0}.z, 1.0)", rawOutput);
                case ConcreteSlotValueType.Vector4:
                    return string.Format("half4({0}.x, {0}.y, {0}.z, 1.0)", rawOutput);
                default:
                    return kErrorString;
            }
        }

        public int numberOfChunks
        {
            get { return m_ShaderChunks.Count; }
        }

        public static string GeneratePreviewShader(AbstractMaterialNode node, out PreviewMode generatedShaderMode)
        {
            generatedShaderMode = PreviewMode.Preview2D;
            return string.Empty;
        }

        /*
        if (!node.GetOutputSlots<MaterialSlot>().Any())
        {
            generatedShaderMode = PreviewMode.Preview2D;
            return string.Empty;
        }

        // figure out what kind of preview we want!
        var activeNodeList = ListPool<INode>.Get();
        NodeUtils.DepthFirstCollectNodesFromNode(activeNodeList, node);
        generatedShaderMode = PreviewMode.Preview2D;

        if (activeNodeList.OfType<AbstractMaterialNode>().Any(x => x.previewMode == PreviewMode.Preview3D))
            generatedShaderMode = PreviewMode.Preview3D;

        string templateLocation = GetTemplatePath("2DPreview.template");
        if (!File.Exists(templateLocation))
            return null;

        string template = File.ReadAllText(templateLocation);

        var shaderBodyVisitor = new ShaderGenerator();
        var shaderFunctionVisitor = new ShaderGenerator();
        var shaderPropertiesVisitor = new PropertyCollector();

        var shaderName = "Hidden/PreviewShader/" + node.GetVariableNameForSlot(node.GetOutputSlots<MaterialSlot>().First().id);

        var owner = node.owner as AbstractMaterialGraph;
        if (owner != null)
            owner.CollectShaderProperties(shaderPropertiesVisitor, GenerationMode.Preview);

        var shaderInputVisitor = new ShaderGenerator();
        var vertexShaderBlock = new ShaderGenerator();

        // always add color because why not.
        shaderInputVisitor.AddShaderChunk("float4 color : COLOR;", true);

        vertexShaderBlock.AddShaderChunk("float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;", true);
        vertexShaderBlock.AddShaderChunk("float3 viewDir = UnityWorldSpaceViewDir(worldPos);", true);
        vertexShaderBlock.AddShaderChunk("float4 screenPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));", true);
        vertexShaderBlock.AddShaderChunk("float3 worldNormal = UnityObjectToWorldNormal(v.normal);", true);

        bool requiresBitangent = activeNodeList.OfType<IMayRequireBitangent>().Any(x => x.RequiresBitangent());
        bool requiresTangent = activeNodeList.OfType<IMayRequireTangent>().Any(x => x.RequiresTangent());
        bool requiresViewDirTangentSpace = activeNodeList.OfType<IMayRequireViewDirectionTangentSpace>().Any(x => x.RequiresViewDirectionTangentSpace());
        bool requiresViewDir = activeNodeList.OfType<IMayRequireViewDirection>().Any(x => x.RequiresViewDirection());
        bool requiresWorldPos = activeNodeList.OfType<IMayRequireWorldPosition>().Any(x => x.RequiresWorldPosition());
        bool requiresNormal = activeNodeList.OfType<IMayRequireNormal>().Any(x => x.RequiresNormal());
        bool requiresScreenPosition = activeNodeList.OfType<IMayRequireScreenPosition>().Any(x => x.RequiresScreenPosition());
        bool requiresVertexColor = activeNodeList.OfType<IMayRequireVertexColor>().Any(x => x.RequiresVertexColor());

        // view directions calculated from world position
        if (requiresWorldPos || requiresViewDir || requiresViewDirTangentSpace)
        {
            shaderInputVisitor.AddShaderChunk("float3 worldPos : TEXCOORD0;", true);
            vertexShaderBlock.AddShaderChunk("o.worldPos = worldPos;", true);
            shaderBodyVisitor.AddShaderChunk("float3 " + ShaderGeneratorNames.Position + " = IN.worldPos;", true);
        }
/*
        if (requiresBitangent || requiresNormal || requiresViewDirTangentSpace)
        {
            shaderInputVisitor.AddShaderChunk("float3 worldNormal : TEXCOORD1;", true);
            vertexShaderBlock.AddShaderChunk("o.worldNormal = worldNormal;", true);
            shaderBodyVisitor.AddShaderChunk("float3 " + ShaderGeneratorNames.Normal + " = normalize(IN.worldNormal);", true);
        }*

        for (int uvIndex = 0; uvIndex < ShaderGeneratorNames.UVCount; ++uvIndex)
        {
            var channel = (UVChannel)uvIndex;
            if (activeNodeList.OfType<IMayRequireMeshUV>().Any(x => x.RequiresMeshUV(channel)))
            {
                shaderInputVisitor.AddShaderChunk(string.Format("half4 meshUV{0} : TEXCOORD{1};", uvIndex, (uvIndex + 5)), true);
                vertexShaderBlock.AddShaderChunk(string.Format("o.meshUV{0} = v.texcoord{1};", uvIndex, uvIndex == 0 ? "" : uvIndex.ToString()), true);
                shaderBodyVisitor.AddShaderChunk(string.Format("half4 {0} = IN.meshUV{1};", channel.GetUVName(), uvIndex), true);
            }
        }

        if (requiresViewDir || requiresViewDirTangentSpace)
        {
            shaderBodyVisitor.AddShaderChunk(
                "float3 "
                + ShaderGeneratorNames.WorldSpaceViewDirection
                + " = normalize(UnityWorldSpaceViewDir("
                + ShaderGeneratorNames.Position
                + "));", true);
        }

        if (requiresScreenPosition)
        {
            shaderInputVisitor.AddShaderChunk("float4 screenPos : TEXCOORD3;", true);
            vertexShaderBlock.AddShaderChunk("o.screenPos = screenPos;", true);
            shaderBodyVisitor.AddShaderChunk("half4 " + ShaderGeneratorNames.ScreenPosition + " = IN.screenPos;", true);
        }

       /* if (requiresBitangent || requiresViewDirTangentSpace || requiresTangent)
        {
            shaderInputVisitor.AddShaderChunk("float4 worldTangent : TEXCOORD4;", true);
            vertexShaderBlock.AddShaderChunk("o.worldTangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);", true);
            shaderBodyVisitor.AddShaderChunk("float3 " + ShaderGeneratorNames.Tangent + " = normalize(IN.worldTangent.xyz);", true);
        }*

        if (requiresBitangent || requiresViewDirTangentSpace)
        {
            shaderBodyVisitor.AddShaderChunk(string.Format("float3 {0} = cross({1}, {2}) * IN.worldTangent.w;", ShaderGeneratorNames.BiTangent, ShaderGeneratorNames.Normal, ShaderGeneratorNames.Tangent), true);
        }

        if (requiresVertexColor)
        {
            vertexShaderBlock.AddShaderChunk("o.color = v.color;", true);
            shaderBodyVisitor.AddShaderChunk("float4 " + ShaderGeneratorNames.VertexColor + " = IN.color;", true);
        }

        var generationMode = GenerationMode.Preview;
        foreach (var activeNode in activeNodeList.OfType<AbstractMaterialNode>())
        {
            if (activeNode is IGeneratesFunction)
                (activeNode as IGeneratesFunction).GenerateNodeFunction(shaderFunctionVisitor, generationMode);
            if (activeNode is IGeneratesBodyCode)
                (activeNode as IGeneratesBodyCode).GenerateNodeCode(shaderBodyVisitor, generationMode);

            activeNode.CollectShaderProperties(shaderPropertiesVisitor, generationMode);
        }


        shaderBodyVisitor.AddShaderChunk("return " + AdaptNodeOutputForPreview(node, node.GetOutputSlots<MaterialSlot>().First().id) + ";", true);

        ListPool<INode>.Release(activeNodeList);

        template = template.Replace("${ShaderName}", shaderName);
        template = template.Replace("${ShaderPropertiesHeader}", shaderPropertiesVisitor.GetPropertiesBlock(2));
        template = template.Replace("${ShaderPropertyUsages}", shaderPropertiesVisitor.GetPropertiesDeclaration(3));
        template = template.Replace("${ShaderInputs}", shaderInputVisitor.GetShaderString(4));
        template = template.Replace("${ShaderFunctions}", shaderFunctionVisitor.GetShaderString(3));
        template = template.Replace("${VertexShaderBody}", vertexShaderBlock.GetShaderString(4));
        template = template.Replace("${PixelShaderBody}", shaderBodyVisitor.GetShaderString(4));

        string vertexShaderBody = vertexShaderBlock.GetShaderString(4);
        if (vertexShaderBody.Length > 0)
        {
            template = template.Replace("${VertexShaderDecl}", "vertex:vert");
            template = template.Replace("${VertexShaderBody}", vertexShaderBody);
        }
        else
        {
            template = template.Replace("${VertexShaderDecl}", "");
            template = template.Replace("${VertexShaderBody}", vertexShaderBody);
        }

        return Regex.Replace(template, @"\r\n|\n\r|\n|\r", Environment.NewLine);
    }*/
    }
}
