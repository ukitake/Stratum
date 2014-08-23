#include "GBufferCommon.fx"

/////////////////////////////////////////////////////
// Clear
/////////////////////////////////////////////////////

struct ClearVSIn
{
	float3 pos : POSITION;
};

struct ClearVSOut
{
	float4 pos : SV_POSITION;
	float3 vpos : TEXCOORD0;
};

ClearVSOut VS_Clear(ClearVSIn input)
{
	// pass through vertices... they should already be in 
	// homogeneous clip space
	ClearVSOut output;
	output.pos = float4(input.pos, 1);
	output.vpos = float3(0, 0, 0);
	return output;
}

GBuffer PS_Clear(ClearVSOut input)
{
	GBuffer output;
	output.Depth = 1.0;
	output.Normal = float4(0, 0, 0, 0);
	output.Albedo = float4(0, 0, 0, 1);
	return output;
}

/////////////////////////////////////////////////////
// Directional Light
/////////////////////////////////////////////////////

struct VS_IN_DLight 
{
	float3 pos : POSITION;
	float3 texAndCornerIndex : TEXCOORD;
};

struct VS_OUT_DLight
{
	float4 pos : SV_Position;
	float2 tex : TEXCOORD0;
	float3 frustumCornerViewSpace : TEXCOORD1;
};

VS_OUT_DLight VS_DLight(VS_IN_DLight input)
{
	VS_OUT_DLight output;
	output.pos = float4(input.pos, 1);
	output.tex = input.texAndCornerIndex.xy;
	output.frustumCornerViewSpace = frustumCornersVS[input.texAndCornerIndex.z];
	return output;
}

float4 PS_DLight(VS_OUT_DLight input) : SV_Target0
{
	float4 normal = gbNormalTexture.Sample(gbNormalSampler, input.tex);
	float depth = gbDepthTexture.Sample(gbDepthSampler, input.tex);
	float4 albedo = gbAlbedoTexture.Sample(gbAlbedoSampler, input.tex);

	if (depth == 1.0){
		clip(-1);
	}

	// multiply view space depth by a ray that passes through this pixel on 
	// its way to the far clip plane
	// TODO THIS IS WRONG... frustum should not be in world space but it is
	float3 vsPosition = depth * far_plane * normalize(input.frustumCornerViewSpace);

	float3 color = saturate(dot(-dLightDirection, normal)) * albedo.rgb * (vsPosition / vsPosition);
	return float4(color, 1.0);
}

technique Clear
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_Clear;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_Clear;
		ComputeShader = null;
	}
}

technique DirectionalLight
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_DLight;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_DLight;
		ComputeShader = null;
	}
}

