Shader "HDRenderPipeline/TerrainLit"
{
    Properties
    {
        [HideInInspector] _Control0("Control 0 (RGBA)", 2D) = "" {}
        [HideInInspector] _Control1("Control 1 (RGBA)", 2D) = "" {}

        [HideInInspector] _Splat0("Layer 0", 2D) = "white" {}
        [HideInInspector] _Splat1("Layer 1", 2D) = "white" {}
        [HideInInspector] _Splat2("Layer 2", 2D) = "white" {}
        [HideInInspector] _Splat3("Layer 3", 2D) = "white" {}
        [HideInInspector] _Splat4("Layer 4", 2D) = "white" {}
        [HideInInspector] _Splat5("Layer 5", 2D) = "white" {}
        [HideInInspector] _Splat6("Layer 6", 2D) = "white" {}
        [HideInInspector] _Splat7("Layer 7", 2D) = "white" {}

        [HideInInspector] _Normal0("Normal 0", 2D) = "bump" {}
        [HideInInspector] _Normal1("Normal 1", 2D) = "bump" {}
        [HideInInspector] _Normal2("Normal 2", 2D) = "bump" {}
        [HideInInspector] _Normal3("Normal 3", 2D) = "bump" {}
        [HideInInspector] _Normal4("Normal 4", 2D) = "bump" {}
        [HideInInspector] _Normal5("Normal 5", 2D) = "bump" {}
        [HideInInspector] _Normal6("Normal 6", 2D) = "bump" {}
        [HideInInspector] _Normal7("Normal 7", 2D) = "bump" {}

        // Since we don't use a mask texture for getting the mask, we'll need the metallic property to be serialized as in sRGB space.
        [HideInInspector] [Gamma] _Metallic0("Metallic 0", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic1("Metallic 1", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic2("Metallic 2", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic3("Metallic 3", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic4("Metallic 4", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic5("Metallic 5", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic6("Metallic 6", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic7("Metallic 7", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness0("Smoothness 0", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness1("Smoothness 1", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness2("Smoothness 2", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness3("Smoothness 3", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness4("Smoothness 4", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness5("Smoothness 5", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness6("Smoothness 6", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness7("Smoothness 7", Range(0.0, 1.0)) = 0.0

        [HideInInspector] [Enum(Alpha, 0, Density, 1, Height, 2)] _LayerBlendMode("Layer Blend Mode", Int) = 0

        [HideInInspector] _Mask0("Mask 0", 2D) = "black" {}
        [HideInInspector] _Mask1("Mask 1", 2D) = "black" {}
        [HideInInspector] _Mask2("Mask 2", 2D) = "black" {}
        [HideInInspector] _Mask3("Mask 3", 2D) = "black" {}
        [HideInInspector] _Mask4("Mask 4", 2D) = "black" {}
        [HideInInspector] _Mask5("Mask 5", 2D) = "black" {}
        [HideInInspector] _Mask6("Mask 6", 2D) = "black" {}
        [HideInInspector] _Mask7("Mask 7", 2D) = "black" {}

        [HideInInspector] _Density0("_Density0", Float) = 1.0
        [HideInInspector] _Density1("_Density1", Float) = 1.0
        [HideInInspector] _Density2("_Density2", Float) = 1.0
        [HideInInspector] _Density3("_Density3", Float) = 1.0
        [HideInInspector] _Density4("_Density4", Float) = 1.0
        [HideInInspector] _Density5("_Density5", Float) = 1.0
        [HideInInspector] _Density6("_Density6", Float) = 1.0
        [HideInInspector] _Density7("_Density7", Float) = 1.0

        // Height in mask.r
        _HeightTransition("Height Transition", Range(0, 1.0)) = 0.0
        [HideInInspector] _HeightAmplitude0("Height Scale0", Float) = 0.02
        [HideInInspector] _HeightAmplitude1("Height Scale1", Float) = 0.02
        [HideInInspector] _HeightAmplitude2("Height Scale2", Float) = 0.02
        [HideInInspector] _HeightAmplitude3("Height Scale3", Float) = 0.02
        [HideInInspector] _HeightAmplitude4("Height Scale4", Float) = 0.02
        [HideInInspector] _HeightAmplitude5("Height Scale5", Float) = 0.02
        [HideInInspector] _HeightAmplitude6("Height Scale6", Float) = 0.02
        [HideInInspector] _HeightAmplitude7("Height Scale7", Float) = 0.02
        [HideInInspector] _HeightCenter0("Height Bias0", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter1("Height Bias1", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter2("Height Bias2", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter3("Height Bias3", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter4("Height Bias4", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter5("Height Bias5", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter6("Height Bias6", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _HeightCenter7("Height Bias7", Range(0.0, 1.0)) = 0.5

        // TODO: support tri-planar?
        // TODO: support more maps?
        /*
        [HideInInspector] _TexWorldScale0("Tiling", Float) = 1.0
        [HideInInspector] _TexWorldScale1("Tiling", Float) = 1.0
        [HideInInspector] _TexWorldScale2("Tiling", Float) = 1.0
        [HideInInspector] _TexWorldScale3("Tiling", Float) = 1.0

        // Following set of parameters represent the parameters node inside the MaterialGraph.
        // They are use to fill a SurfaceData. With a MaterialGraph this should not exist.

        // All the following properties are filled by the referenced lit shader.

        _SmoothnessRemapMin0("SmoothnessRemapMin0", Range(0.0, 1.0)) = 0.0
        _SmoothnessRemapMin1("SmoothnessRemapMin1", Range(0.0, 1.0)) = 0.0
        _SmoothnessRemapMin2("SmoothnessRemapMin2", Range(0.0, 1.0)) = 0.0
        _SmoothnessRemapMin3("SmoothnessRemapMin3", Range(0.0, 1.0)) = 0.0

        _SmoothnessRemapMax0("SmoothnessRemapMax0", Range(0.0, 1.0)) = 1.0
        _SmoothnessRemapMax1("SmoothnessRemapMax1", Range(0.0, 1.0)) = 1.0
        _SmoothnessRemapMax2("SmoothnessRemapMax2", Range(0.0, 1.0)) = 1.0
        _SmoothnessRemapMax3("SmoothnessRemapMax3", Range(0.0, 1.0)) = 1.0

        _AORemapMin0("AORemapMin0", Range(0.0, 1.0)) = 0.0
        _AORemapMin1("AORemapMin1", Range(0.0, 1.0)) = 0.0
        _AORemapMin2("AORemapMin2", Range(0.0, 1.0)) = 0.0
        _AORemapMin3("AORemapMin3", Range(0.0, 1.0)) = 0.0

        _AORemapMax0("AORemapMax0", Range(0.0, 1.0)) = 1.0
        _AORemapMax1("AORemapMax1", Range(0.0, 1.0)) = 1.0
        _AORemapMax2("AORemapMax2", Range(0.0, 1.0)) = 1.0
        _AORemapMax3("AORemapMax3", Range(0.0, 1.0)) = 1.0
        */
        // All the following properties exist only in layered lit material

        // Following are builtin properties

        // Stencil state
        [HideInInspector] _StencilRef("_StencilRef", Int) = 2 // StencilLightingUsage.RegularLighting
        [HideInInspector] _StencilWriteMask("_StencilWriteMask", Int) = 7 // StencilMask.Lighting  (fixed at compile time)
        [HideInInspector] _StencilRefMV("_StencilRefMV", Int) = 128 // StencilLightingUsage.RegularLighting  (fixed at compile time)
        [HideInInspector] _StencilWriteMaskMV("_StencilWriteMaskMV", Int) = 128 // StencilMask.ObjectsVelocity  (fixed at compile time)

        // Blending state
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector] _CullMode("__cullmode", Float) = 2.0
        [HideInInspector] _ZTestDepthEqualForOpaque("_ZTestDepthEqualForOpaque", Int) = 4 // Less equal
        [HideInInspector] _ZTestGBuffer("_ZTestGBuffer", Int) = 4

        [ToggleUI] _DoubleSidedEnable("Double sided enable", Float) = 0.0
        [Enum(Flip, 0, Mirror, 1, None, 2)] _DoubleSidedNormalMode("Double sided normal mode", Float) = 1
        [HideInInspector] _DoubleSidedConstants("_DoubleSidedConstants", Vector) = (1, 1, -1, 0)

        [ToggleUI] _EnableInstancedPerPixelNormal("Instanced per pixel normal", Float) = 1.0

        // Caution: C# code in BaseLitUI.cs call LightmapEmissionFlagsProperty() which assume that there is an existing "_EmissionColor"
        // value that exist to identify if the GI emission need to be enabled.
        // In our case we don't use such a mechanism but need to keep the code quiet. We declare the value and always enable it.
        // TODO: Fix the code in legacy unity so we can customize the behavior for GI
        _EmissionColor("Color", Color) = (1, 1, 1)

        // HACK: GI Baking system relies on some properties existing in the shader ("_MainTex", "_Cutoff" and "_Color") for opacity handling, so we need to store our version of those parameters in the hard-coded name the GI baking system recognizes.
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        [ToggleUI] _SupportDBuffer("Support DBuffer", Float) = 1.0
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal

    #pragma shader_feature _TERRAIN_8_LAYERS
    #pragma shader_feature _ _TERRAIN_BLEND_DENSITY _TERRAIN_BLEND_HEIGHT
    #pragma shader_feature _NORMALMAP
    #pragma shader_feature _MASKMAP
    // Sample normal in pixel shader when doing instancing
    #pragma shader_feature _TERRAIN_INSTANCED_PERPIXEL_NORMAL

    #pragma shader_feature _DOUBLESIDED_ON

    //#pragma shader_feature _ _LAYER_MAPPING_PLANAR0 _LAYER_MAPPING_TRIPLANAR0
    //#pragma shader_feature _ _LAYER_MAPPING_PLANAR1 _LAYER_MAPPING_TRIPLANAR1
    //#pragma shader_feature _ _LAYER_MAPPING_PLANAR2 _LAYER_MAPPING_TRIPLANAR2
    //#pragma shader_feature _ _LAYER_MAPPING_PLANAR3 _LAYER_MAPPING_TRIPLANAR3

    #pragma shader_feature _DISABLE_DBUFFER

    //enable GPU instancing support
    #pragma multi_compile_instancing
    #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

    //-------------------------------------------------------------------------------------
    // Define
    //-------------------------------------------------------------------------------------

    #define UNITY_MATERIAL_LIT // Need to be define before including Material.hlsl
    // Use surface gradient normal mapping as it handle correctly triplanar normal mapping and multiple UVSet
    #define SURFACE_GRADIENT
    // This shader support vertex modification
    #define HAVE_VERTEX_MODIFICATION

    //-------------------------------------------------------------------------------------
    // Include
    //-------------------------------------------------------------------------------------

    #include "CoreRP/ShaderLibrary/Common.hlsl"
    #include "../../ShaderPass/FragInputs.hlsl"
    #include "../../ShaderPass/ShaderPass.cs.hlsl"

    //-------------------------------------------------------------------------------------
    // variable declaration
    //-------------------------------------------------------------------------------------

    #include "../../Material/Lit/LitProperties.hlsl"

    // All our shaders use same name for entry point
    #pragma vertex Vert
    #pragma fragment Frag

    ENDHLSL

    SubShader
    {
        // This tags allow to use the shader replacement features
        Tags{ "RenderPipeline" = "HDRenderPipeline" "RenderType" = "Opaque" "SplatCount"="8" }

        // Caution: The outline selection in the editor use the vertex shader/hull/domain shader of the first pass declare. So it should not bethe  meta pass.
        Pass
        {
            Name "GBuffer"  // Name is not used
            Tags { "LightMode" = "GBuffer" } // This will be only for opaque object based on the RenderQueue index

            Cull [_CullMode]
            ZTest[_ZTestGBuffer]

            Stencil
            {
                WriteMask [_StencilWriteMask]
                Ref [_StencilRef]
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM

            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            #define SHADERPASS SHADERPASS_GBUFFER
            #include "../../ShaderVariables.hlsl"
            #ifdef DEBUG_DISPLAY
            #include "../../Debug/DebugDisplay.hlsl"
            #endif
            #include "../../Material/Material.hlsl"
            #include "TerrainLitSharePass.hlsl"
            #include "TerrainLitData.hlsl"
            #include "../../ShaderPass/ShaderPassGBuffer.hlsl"

            ENDHLSL
        }

        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags{ "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM

            // Lightmap memo
            // DYNAMICLIGHTMAP_ON is used when we have an "enlighten lightmap" ie a lightmap updated at runtime by enlighten.This lightmap contain indirect lighting from realtime lights and realtime emissive material.Offline baked lighting(from baked material / light,
            // both direct and indirect lighting) will hand up in the "regular" lightmap->LIGHTMAP_ON.

            #define SHADERPASS SHADERPASS_LIGHT_TRANSPORT
            #include "../../ShaderVariables.hlsl"
            #include "../../Material/Material.hlsl"
            #include "TerrainLitSharePass.hlsl"
            #include "TerrainLitData.hlsl"
            #include "../../ShaderPass/ShaderPassLightTransport.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Motion Vectors"
            Tags{ "LightMode" = "MotionVectors" } // Caution, this need to be call like this to setup the correct parameters by C++ (legacy Unity)

            // If velocity pass (motion vectors) is enabled we tag the stencil so it don't perform CameraMotionVelocity
            Stencil
            {
                WriteMask [_StencilWriteMaskMV]
                Ref [_StencilRefMV]
                Comp Always
                Pass Replace
            }

            Cull[_CullMode]

            ZWrite On

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_VELOCITY
            #include "../../ShaderVariables.hlsl"
            #include "../../Material/Material.hlsl"
            #include "../Lit/ShaderPass/LitVelocityPass.hlsl"
            #include "TerrainLitData.hlsl"
            #include "../../ShaderPass/ShaderPassVelocity.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }

            Cull[_CullMode]

            ZClip [_ZClip]
            ZWrite On
            ZTest LEqual

            ColorMask 0

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_SHADOWS
            #define USE_LEGACY_UNITY_MATRIX_VARIABLES
            #include "../../ShaderVariables.hlsl"
            #include "../../Material/Material.hlsl"
            #include "../Lit/ShaderPass/LitDepthPass.hlsl"
            #include "TerrainLitData.hlsl"
            #include "../../ShaderPass/ShaderPassDepthOnly.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{ "LightMode" = "DepthOnly" }

            Cull[_CullMode]

            ZWrite On

            ColorMask 0

            HLSLPROGRAM

            #define SHADERPASS SHADERPASS_DEPTH_ONLY
            #include "../../ShaderVariables.hlsl"
            #include "../../Material/Material.hlsl"
            #include "../Lit/ShaderPass/LitDepthPass.hlsl"
            #include "TerrainLitData.hlsl"
            #include "../../ShaderPass/ShaderPassDepthOnly.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Forward" // Name is not used
            Tags{ "LightMode" = "Forward" } // This will be only for transparent object based on the RenderQueue index

            Stencil
            {
                WriteMask [_StencilWriteMask]
                Ref [_StencilRef]
                Comp Always
                Pass Replace
            }

            // In case of forward we want to have depth equal for opaque mesh
            ZTest [_ZTestDepthEqualForOpaque]
            ZWrite [_ZWrite]
            Cull [_CullMode]

            HLSLPROGRAM

            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            // #include "../../Lighting/Forward.hlsl"
            //#pragma multi_compile LIGHTLOOP_SINGLE_PASS LIGHTLOOP_TILE_PASS
            #define LIGHTLOOP_TILE_PASS
            #pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST

            #define SHADERPASS SHADERPASS_FORWARD
            // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
            #ifndef _SURFACE_TYPE_TRANSPARENT
                #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
            #endif
            #include "../../ShaderVariables.hlsl"
            #ifdef DEBUG_DISPLAY
            #include "../../Debug/DebugDisplay.hlsl"
            #endif
            #include "../../Lighting/Lighting.hlsl"
            #include "TerrainLitSharePass.hlsl"
            #include "TerrainLitData.hlsl"
            #include "../../ShaderPass/ShaderPassForward.hlsl"

            ENDHLSL
        }

        UsePass "Hidden/Nature/Terrain/Picking/TERRAINPICKING"
    }

    Dependency "BaseMapShader" = "Hidden/HDRenderPipeline/TerrainLit_Basemap"
    Dependency "BaseMapGenShader" = "Hidden/HDRenderPipeline/TerrainLit_Basemap_Gen"
    CustomEditor "UnityEditor.Experimental.Rendering.HDPipeline.TerrainLitGUI"
}
