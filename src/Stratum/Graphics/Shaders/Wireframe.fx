struct VS_IN_TERRAIN
{
	float3 pos : POSITION;
	float3 tex : TEXCOORD0;
	float4 texBounds : TEXCOORD1;
};

struct VS_OUT_TERRAIN
{
	float3 pos : POSITION;
	float3 tex : TEXCOORD0;
	float4 texBounds : TEXCOORD1;
};

struct PS_IN_TERRAIN 
{
	float4 pos : SV_POSITION;
	float3 wpos : TEXCOORD0;
	float3 vpos : TEXCOORD1;
	float3 tex : TEXCOORD2;
	float4 texBounds : TEXCOORD3;
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
	float4x4 Proj;

	// material parameters
};

static const float PI = 3.14159265f;

VS_OUT_TERRAIN VS_TERRAIN(VS_IN_TERRAIN input)
{
	VS_OUT_TERRAIN output;
	output.pos = input.pos;
	output.tex = input.tex;
	output.texBounds = input.texBounds;
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
	// VS_OUT_POS_TESS is exactly the structure we need here... reuse it
	VS_OUT_TERRAIN output;
	output.pos = inputPatch[pointId].pos;
	output.tex = inputPatch[pointId].tex;
	output.texBounds = inputPatch[pointId].texBounds;
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
	float lat = vertexPosition.y * (PI / 180.0);
	float lon = vertexPosition.x * (PI / 180.0);

	// lat long to cartesian
	float3 ret;
    ret.x = 6360.0 * cos(lat) * cos(lon);
	ret.y = 6360.0 * sin(lat);
	ret.z = 6360.0 * cos(lat) * sin(lon);

	ret = mul(float4(ret, 1.0), World).xyz;
	output.wpos = ret;
	output.vpos = mul(float4(output.wpos, 1.0), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.tex = float3(texCoord, patch[0].tex.z);
	output.texBounds = patch[0].texBounds;
	return output;
}

Texture2DArray gpuTileCache;
SamplerState cacheSampler;

float4 PS_TERRAIN(PS_IN_TERRAIN input) : SV_Target0
{
	float2 tx = float2(asin(input.wpos.y / 6360.0), atan2(input.wpos.z, input.wpos.x));
	float2 txDeg = tx * 180 / 3.14159265f;
	float2 texMerc;
	float lat = txDeg.x;
	float lon = txDeg.y;
	texMerc.x = lon * 20037508.34 / 180;
    texMerc.y = log(tan((90 + lat) * 3.14159265f / 360)) / (3.14159265f / 180);
    texMerc.y = texMerc.y * 20037508.34 / 180;

	float2 minBounds = input.texBounds.xy;
	float2 maxBounds = input.texBounds.zw;

	float2 texCoords = float2((texMerc.x - minBounds.x) / (maxBounds.x - minBounds.x), 1 - (texMerc.y - minBounds.y) / (maxBounds.y - minBounds.y));

	//return float4(input.tex.x, input.tex.y, input.tex.z, 1);
	return gpuTileCache.Sample(cacheSampler, float3(texCoords, input.tex.z));
	//return float4(texCoords.x, texCoords.y, 0, 1);
}

technique Terrain
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_TERRAIN;
		HullShader = HS_TERRAIN;
		DomainShader = DS_TERRAIN;
		GeometryShader = null;
		PixelShader = PS_TERRAIN;
		ComputeShader = null;
	}
}