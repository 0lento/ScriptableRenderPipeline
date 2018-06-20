#if SHADERPASS != SHADERPASS_GBUFFER
#error SHADERPASS_is_not_correctly_define
#endif

#include "VertMesh.hlsl"

PackedVaryingsType Vert(AttributesMesh inputMesh)
{
    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(inputMesh);
    return PackVaryingsType(varyingsType);
}

#ifdef TESSELLATION_ON

PackedVaryingsToPS VertTesselation(VaryingsToDS input)
{
    VaryingsToPS output;
    output.vmesh = VertMeshTesselation(input.vmesh);
    return PackVaryingsToPS(output);
}

#include "TessellationShare.hlsl"

#endif // TESSELLATION_ON

//forest-begin: G-Buffer motion vectors
#if defined(HAS_VEGETATION_ANIM) && defined(GBUFFER_MOTION_VECTORS)
	float2 _CalculateVelocity(float4 positionCS, float4 previousPositionCS)
	{
		// Encode velocity
		positionCS.xy = positionCS.xy / positionCS.w;
		previousPositionCS.xy = previousPositionCS.xy / previousPositionCS.w;

		// Work around mismatch between object and fullscreen velocity outputs.
		positionCS = (positionCS + 1.0) / 2.0;
		previousPositionCS = (previousPositionCS + 1.0) / 2.0;

		float2 velocity = (positionCS.xy - previousPositionCS.xy);
	#if UNITY_UV_STARTS_AT_TOP
		velocity.y = -velocity.y;
	#endif
		return velocity;
	}
#endif
//forest-end:

void Frag(  PackedVaryingsToPS packedInput,
            OUTPUT_GBUFFER(outGBuffer)
            OUTPUT_GBUFFER_SHADOWMASK(outShadowMaskBuffer)
            #ifdef _DEPTHOFFSET_ON
            , out float outputDepth : SV_Depth
            #endif
//forest-begin: G-Buffer motion vectors
#if defined(HAS_VEGETATION_ANIM) && defined(GBUFFER_MOTION_VECTORS)
			, out float2 velocityTexture : SV_Target4
#endif
//forest-end:
            )
{
    FragInputs input = UnpackVaryingsMeshToFragInputs(packedInput.vmesh);

	// input.positionSS is SV_Position
	PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionWS);

//forest-begin: G-Buffer motion vectors
#if defined(HAS_VEGETATION_ANIM) && defined(GBUFFER_MOTION_VECTORS)
	float2 velocity = _CalculateVelocity(packedInput.vmesh.mvPositionCS, packedInput.vmesh.mvPrevPositionCS);

	float4 packedVelocity;
	EncodeVelocity(velocity, packedVelocity);

	velocityTexture = packedVelocity.xy;
#endif
//forest-end:

#ifdef VARYINGS_NEED_POSITION_WS
    float3 V = GetWorldSpaceNormalizeViewDir(input.positionWS);
#else
    float3 V = 0; // Avoid the division by 0
#endif

    SurfaceData surfaceData;
    BuiltinData builtinData;
    GetSurfaceAndBuiltinData(input, V, posInput, surfaceData, builtinData);

#ifdef DEBUG_DISPLAY
    ApplyDebugToSurfaceData(input.worldToTangent, surfaceData);
#endif

    BSDFData bsdfData = ConvertSurfaceDataToBSDFData(surfaceData);

    PreLightData preLightData = GetPreLightData(V, posInput, bsdfData);

    float3 bakeDiffuseLighting = GetBakedDiffuseLighting(surfaceData, builtinData, bsdfData, preLightData);

    ENCODE_INTO_GBUFFER(surfaceData, bakeDiffuseLighting, posInput.positionSS, outGBuffer);
    ENCODE_SHADOWMASK_INTO_GBUFFER(float4(builtinData.shadowMask0, builtinData.shadowMask1, builtinData.shadowMask2, builtinData.shadowMask3), outShadowMaskBuffer);

#ifdef _DEPTHOFFSET_ON
    outputDepth = posInput.deviceDepth;
#endif
}
