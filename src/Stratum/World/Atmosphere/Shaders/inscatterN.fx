#include "Common.fx"

float r;
float4 dhdH;
int layer;

Texture2D trans;
SamplerState transSamp;

Texture3D deltaJ;
SamplerState deltaJSamp;

float3 integrand(float r, float mu, float muS, float nu, float t) {
    float ri = sqrt(r * r + t * t + 2.0 * r * mu * t);
    float mui = (r * mu + t) / ri;
    float muSi = (nu * t + muS * r) / ri;
    return texture4D(deltaJ, deltaJSamp, ri, mui, muSi, nu).rgb * transmittance(trans, transSamp, r, mu, t);
}

float3 inscatter(float r, float mu, float muS, float nu) {
    float3 raymie = float3(0.0, 0.0, 0.0);
    float dx = limit(r, mu) / float(INSCATTER_INTEGRAL_SAMPLES);
    float xi = 0.0;
    float3 raymiei = integrand(r, mu, muS, nu, 0.0);

	[loop]
    for (int i = 1; i <= INSCATTER_INTEGRAL_SAMPLES; ++i) {
        float xj = float(i) * dx;
        float3 raymiej = integrand(r, mu, muS, nu, xj);
        raymie += (raymiei + raymiej) / 2.0 * dx;
        xi = xj;
        raymiei = raymiej;
    }
    return raymie;
}

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
    return float4(inscatter(r, mu, muS, nu), 1.0);
}

technique InscatterN
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