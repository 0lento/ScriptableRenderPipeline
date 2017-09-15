using System.Collections.Generic;

namespace UnityEngine.MaterialGraph
{
    public interface IGenerateProperties
    {
        void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode);
    }
}
