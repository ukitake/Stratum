struct VS_IN_TERRAIN
{
	float3 pos : POSITION;
	float2 tex : TEXCOORD0;
};

struct VS_OUT_TERRAIN
{
	float3 pos : POSITION;
	float2 tex : TEXCOORD0;
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

cbuffer PerObject 
{
	float4x4 World;
	float4x4 View;
	float4x4 ViewNoT;
	float4x4 Proj;
	float4x4 ViewProj;

	float GlobalTime;
	float3 CameraPosition;
	uint3 CameraPositionHigh;
	uint3 CameraPositionLow;
	float2 ViewportSize;
};

static const float PI = 3.14159265f;

VS_OUT_TERRAIN VS_TERRAIN(VS_IN_TERRAIN input)
{
	VS_OUT_TERRAIN output;
	output.pos = input.pos;
	output.tex = input.tex;
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
	output.pos = inputPatch[pointId].pos;
	output.tex = inputPatch[pointId].tex;
	return output;
}

[domain("quad")]
PS_IN_TERRAIN DS_TERRAIN(HS_CONSTANT_TERRAIN input, float2 uvCoord : SV_DomainLocation, const OutputPatch<VS_OUT_TERRAIN, 4> patch)
{
	PS_IN_TERRAIN output;
	float3 topMidpoint = lerp(patch[0].pos, patch[1].pos, uvCoord.x);
	float3 bottomMidpoint = lerp(patch[3].pos, patch[2].pos, uvCoord.x);
	float3 vertexPosition = lerp(topMidpoint, bottomMidpoint, uvCoord.y);

	float2 topTex = lerp(patch[0].tex.xy, patch[1].tex.xy, uvCoord.x);
	float2 botTex = lerp(patch[3].tex.xy, patch[2].tex.xy, uvCoord.x);
	float2 texCoord = lerp(topTex, botTex, uvCoord.y);
	float lat = vertexPosition.y;
	float lon = vertexPosition.x;
	
	// lat long to cartesian
	float3 ret;
    ret.x = cos(lat) * cos(lon);
	ret.y = sin(lat);
	ret.z = cos(lat) * sin(lon);
	ret *= 6360.0;

	float3 wpos = mul(float4(ret, 1.0), World).xyz;
	output.wpos = wpos;
	output.vpos = (float3)mul(float4(wpos, 1.0), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.tex = texCoord;
	return output;
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