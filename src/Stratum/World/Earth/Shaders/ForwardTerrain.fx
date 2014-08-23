#include "../../../Graphics/Shaders/Common.fx"

struct VS_IN_TERRAIN
{
	float3 poshigh : POSITION0;
	float3 poslow : POSITION1;
	float2 tex : TEXCOORD0;
	float split : TEXCOORD1;
};

struct VS_OUT_TERRAIN
{
	float3 poshigh : POSITION0;
	float3 poslow : POSITION1;
	float2 tex : TEXCOORD0;
	float split : TEXCOORD1;
};

struct PS_IN_TERRAIN 
{
	float4 pos : SV_POSITION;
	float3 wpos : TEXCOORD0;
	float3 vpos : TEXCOORD1;
	float2 tex : TEXCOORD2;
};

struct HS_CONSTANT_TERRAIN 
{
	float edges[4] : SV_TessFactor;
	float inside[2] : SV_InsideTessFactor;
};

static const float PI = 3.14159265f;

VS_OUT_TERRAIN VS_TERRAIN(VS_IN_TERRAIN input)
{
	VS_OUT_TERRAIN output;
	output.poshigh = input.poshigh;
	output.poslow = input.poslow;
	output.tex = input.tex;
	output.split = input.split;
	return output;
}

HS_CONSTANT_TERRAIN HS_CON_TERRAIN(InputPatch<VS_OUT_TERRAIN, 4> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_CONSTANT_TERRAIN output;

	float tessAmount = 64;

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = tessAmount;
    output.edges[1] = tessAmount;
    output.edges[2] = tessAmount;
	output.edges[3] = tessAmount;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside[0] = tessAmount;
    output.inside[1] = tessAmount;

    return output;
}

[domain("quad")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(4)]
[patchconstantfunc("HS_CON_TERRAIN")]
VS_OUT_TERRAIN HS_TERRAIN(InputPatch<VS_OUT_TERRAIN, 4> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	VS_OUT_TERRAIN output;
	output.poshigh = inputPatch[pointId].poshigh;
	output.poslow = inputPatch[pointId].poslow;
	output.tex = inputPatch[pointId].tex;
	output.split = inputPatch[pointId].split;
	return output;
}

//float plerp(float2 v0, float2 v1, float t) 
//{
	//return ds_add(ds_muls(1 - t, v0), ds_muls(v1, t));
	//return (1 - t)*v0 + t*v1;
//}

[domain("quad")]
PS_IN_TERRAIN DS_TERRAIN(HS_CONSTANT_TERRAIN input, float2 uvCoord : SV_DomainLocation, const OutputPatch<VS_OUT_TERRAIN, 4> patch)
{
	PS_IN_TERRAIN output;
	float3 tmh = lerp(patch[0].poshigh, patch[1].poshigh, uvCoord.x);
	float3 bmh = lerp(patch[3].poshigh, patch[2].poshigh, uvCoord.x);
	float3 wph = lerp(tmh, bmh, uvCoord.y);

	float3 tml = lerp(patch[0].poslow, patch[1].poslow, uvCoord.x);
	float3 bml = lerp(patch[3].poslow, patch[2].poslow, uvCoord.x);
	float3 wpl = lerp(tml, bml, uvCoord.y);

	float2 topTex = lerp(patch[0].tex.xy, patch[1].tex.xy, uvCoord.x);
	float2 botTex = lerp(patch[3].tex.xy, patch[2].tex.xy, uvCoord.x);
	float2 texCoord = lerp(topTex, botTex, uvCoord.y);

	if (patch[0].split < 16)
	{
		float lat = wph.y;
		float lon = wph.x;

		float3 cartesian;
		cartesian.x = cos(lat) * cos(lon);
		cartesian.y = sin(lat);
		cartesian.z = cos(lat) * sin(lon);
		cartesian *= 6360.0;

		output.wpos = cartesian;
		output.vpos = mul(float4(cartesian, 1.0), View).xyz;
		output.pos = mul(float4(output.vpos, 1.0), Proj);
		output.tex = texCoord;
		return output;
	}
	else
	{
		output.wpos = wph;
		output.vpos = wph;
		output.pos = mul(float4(output.vpos, 1.0), Proj);
		output.tex = texCoord;
		return output;
	}
}

float4 PS_TERRAIN(PS_IN_TERRAIN input) : SV_Target0
{
	return float4(input.tex.x, input.tex.y, 0, 1);
}

technique Terrain
{
	pass Pass0
	{
		Profile = 11;
		VertexShader = VS_TERRAIN;
		HullShader = HS_TERRAIN;
		DomainShader = DS_TERRAIN;
		GeometryShader = null;
		PixelShader = PS_TERRAIN;
		ComputeShader = null;
	}
}