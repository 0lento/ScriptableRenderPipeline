using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Graphing;
using UnityEngine.MaterialGraph;

namespace UnityEditor.MaterialGraph.UnitTests
{
    [TestFixture]
    public class Function4InputTests
    {
        private class Function4InputTestNode : Function4Input, IGeneratesFunction
        {
            public Function4InputTestNode()
            {
                name = "Function4InputTestNode";
            }

            protected override string GetFunctionName()
            {
                return "unity_test_" + precision;
            }

            public void GenerateNodeFunction(ShaderGenerator visitor, GenerationMode generationMode)
            {
                var outputString = new ShaderGenerator();
                outputString.AddShaderChunk(GetFunctionPrototype("arg1", "arg2", "arg3", "arg4"), false);
                outputString.AddShaderChunk("{", false);
                outputString.Indent();
                outputString.AddShaderChunk("return arg1 + arg2 + arg3 + arg4;", false);
                outputString.Deindent();
                outputString.AddShaderChunk("}", false);

                visitor.AddShaderChunk(outputString.GetShaderString(0), true);
            }
        }

        private UnityEngine.MaterialGraph.MaterialGraph m_Graph;
        private Vector1Node m_InputOne;
        private Vector1Node m_InputTwo;
        private Vector1Node m_InputThree;
        private Vector1Node m_InputFour;
        private Function4InputTestNode m_TestNode;

        [TestFixtureSetUp]
        public void RunBeforeAnyTests()
        {
            Debug.unityLogger.logHandler = new ConsoleLogHandler();
        }

        [SetUp]
        public void TestSetUp()
        {
            m_Graph = new UnityEngine.MaterialGraph.MaterialGraph();
            m_InputOne = new Vector1Node();
            m_InputTwo = new Vector1Node();
            m_InputThree = new Vector1Node();
            m_InputFour = new Vector1Node();
            m_TestNode = new Function4InputTestNode();

            m_Graph.AddNode(m_InputOne);
            m_Graph.AddNode(m_InputTwo);
            m_Graph.AddNode(m_InputThree);
            m_Graph.AddNode(m_InputFour);
            m_Graph.AddNode(m_TestNode);
            m_Graph.AddNode(new MetallicMasterNode());

            m_InputOne.value = 0.2f;
            m_InputTwo.value = 0.3f;
            m_InputThree.value = 0.6f;
            m_InputFour.value = 0.6f;

            m_Graph.Connect(m_InputOne.GetSlotReference(Vector1Node.OutputSlotId), m_TestNode.GetSlotReference(Function4Input.InputSlot1Id));
            m_Graph.Connect(m_InputTwo.GetSlotReference(Vector1Node.OutputSlotId), m_TestNode.GetSlotReference(Function4Input.InputSlot2Id));
            m_Graph.Connect(m_InputThree.GetSlotReference(Vector1Node.OutputSlotId), m_TestNode.GetSlotReference(Function4Input.InputSlot3Id));
            m_Graph.Connect(m_InputThree.GetSlotReference(Vector1Node.OutputSlotId), m_TestNode.GetSlotReference(Function4Input.InputSlot4Id));
            m_Graph.Connect(m_TestNode.GetSlotReference(Function4Input.OutputSlotId), m_Graph.masterNode.GetSlotReference(MetallicMasterNode.NormalSlotId));
        }

        [Test]
        public void TestGenerateNodeCodeGeneratesCorrectCode()
        {
            string expected = string.Format("half {0} = unity_test_half ({1}, {2}, {3}, {4});{5}"
                    , m_TestNode.GetVariableNameForSlot(Function4Input.OutputSlotId)
                    , m_InputOne.GetVariableNameForSlot(Vector1Node.OutputSlotId)
                    , m_InputTwo.GetVariableNameForSlot(Vector1Node.OutputSlotId)
                    , m_InputThree.GetVariableNameForSlot(Vector1Node.OutputSlotId)
                    , m_InputFour.GetVariableNameForSlot(Vector1Node.OutputSlotId)
                    , Environment.NewLine);

            ShaderGenerator visitor = new ShaderGenerator();
            m_TestNode.GenerateNodeCode(visitor, GenerationMode.ForReals);
            Assert.AreEqual(expected, visitor.GetShaderString(0));
        }

        [Test]
        public void TestGenerateNodeFunctionGeneratesCorrectCode()
        {
            string expected =
                "inline half unity_test_half (half arg1, half arg2, half arg3, half arg4)" + Environment.NewLine
                + "{" + Environment.NewLine
                + "\treturn arg1 + arg2 + arg3 + arg4;" + Environment.NewLine
                + "}" + Environment.NewLine;

            ShaderGenerator visitor = new ShaderGenerator();
            m_TestNode.GenerateNodeFunction(visitor, GenerationMode.ForReals);
            Assert.AreEqual(expected, visitor.GetShaderString(0));
        }
    }
}
