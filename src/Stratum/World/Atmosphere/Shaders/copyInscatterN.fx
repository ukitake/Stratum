#include "Common.fx"

float r;
float4 dhdH;
int layer;

Texture3D deltaS;
SamplerState deltaSSamp;

struct VS_OUT 
{
	float4 position : SV_POSITION;
};

VS_OUT VS(float3 pos : POSITION)
{
	VS_OUT o;
	o.position = float4(pos, 1);
	return o;
}

struct GS_OUT 
{
	float4 PosH : SV_POSITION;
	float2 Tex : TEXCOORD0;
	uint RTIndex : SV_RenderTargetArrayIndex;
};

[maxvertexcount(3)]
void GS(triangle VS_OUT In[3], inout TriangleStream<GS_OUT> triStream)
{
	GS_OUT o;
	
	for (uint i = 0; i < 3; i++)
	{
		o.PosH = In[i].position;
		o.Tex = In[i].position.xy * 0.5 + float2(0.5, 0.5);
		o.Tex.y = 1 - o.Tex.y;
		o.RTIndex = layer;
		triStream.Append(o);
	}
}

float4 PS(GS_OUT o) : SV_TARGET0
{
	float mu, muS, nu;
    getMuMuSNu(float2(o.Tex.x * RES_MU_S * RES_NU, o.Tex.y * RES_MU), r, dhdH, mu, muS, nu);
    float3 uvw = float3(o.Tex.x, o.Tex.y, float(layer) / float(RES_R));
    return float4(deltaS.Sample(deltaSSamp, uvw).rgb / phaseFunctionR(nu), 0.0);
}

technique CopyInscatterN
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS;
		HullShader = null;
		DomainShader = null;
		GeometryShader = GS;
		ComputeShader = null;
		PixelShader = PS;
	}
}