struct VS_IN
{
	float3 pos : POSITION;
};

struct VS_OUT
{
	float3 pos : POSITION;
};

struct PS_IN 
{
	float4 pos : SV_POSITION;
	float3 wpos : TEXCOORD0;
};

struct HS_CONSTANT 
{
	float edges[3] : SV_TessFactor;
	float inside : SV_InsideTessFactor;
};

cbuffer PerObject 
{
	float4x4 World;
	float4x4 View;
	float4x4 ViewNoT;
	float4x4 Proj;

	// material parameters
};

static const float PI = 3.14159265f;

VS_OUT VS(VS_IN input)
{
	VS_OUT output;
	output.pos = input.pos;
	return output;
}

HS_CONSTANT HS_CON(InputPatch<VS_OUT, 3> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_CONSTANT output;

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = 20.0;
    output.edges[1] = 20.0;
    output.edges[2] = 20.0;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside = 20.0;

    return output;
}

[domain("tri")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("HS_CON")]
VS_OUT HS(InputPatch<VS_OUT, 3> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	// VS_OUT_POS_TESS is exactly the structure we need here... reuse it
	VS_OUT output;
	output.pos = inputPatch[pointId].pos;
	return output;
}

[domain("tri")]
PS_IN DS(HS_CONSTANT input, float3 uvwCoord : SV_DomainLocation, const OutputPatch<VS_OUT, 3> patch)
{
	PS_IN output;
	float3 vertexPosition = uvwCoord.x * patch[0].pos + uvwCoord.y * patch[1].pos + uvwCoord.z * patch[2].pos;
	vertexPosition = normalize(vertexPosition);
	float3 vpos = mul(float4(vertexPosition, 1), ViewNoT).xyz;
	output.pos = mul(float4(vpos, 1), Proj);
	output.wpos = vertexPosition;
	return output;
}

Texture2D skyboxTex;
SamplerState skyboxSampler;

static const float EXPOSURE = 1.5;

float3 HDR(float3 color)
{
	return 1.0f - exp(-EXPOSURE * color);
}

float4 PS(PS_IN input) : SV_Target0
{
	float2 tx = float2(asin(input.wpos.y), atan2(input.wpos.z, input.wpos.x));
	float2 txDeg = tx * 180 / 3.14159265f;
	float2 texMerc;
	float lat = txDeg.x;
	float lon = txDeg.y;

	float2 texCoords = float2((lon + 180) / 360, (lat + 90) / 180);

	//return float4(input.tex.x, input.tex.y, input.tex.z, 1);
	return float4(skyboxTex.Sample(skyboxSampler, texCoords).rgb, 1.0);
	//return float4(texCoords.x, texCoords.y, 0, 1);
}

technique Skybox
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS;
		HullShader = HS;
		DomainShader = DS;
		GeometryShader = null;
		PixelShader = PS;
		ComputeShader = null;
	}
}