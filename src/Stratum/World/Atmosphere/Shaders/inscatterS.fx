#include "Common.fx"

float r;
float4 dhdH;
int layer;

float first;

Texture2D trans;
SamplerState transSamp;

Texture2D deltaE;
SamplerState deltaESamp;

Texture3D deltaSR;
SamplerState deltaSRSamp;

Texture3D deltaSM;
SamplerState deltaSMSamp;

static const float dphi = float(M_PI) / float(INSCATTER_SPHERICAL_INTEGRAL_SAMPLES);
static const float dtheta = float(M_PI) / float(INSCATTER_SPHERICAL_INTEGRAL_SAMPLES);

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

void inscatter(float r, float mu, float muS, float nu, out float3 raymie) {
    r = clamp(r, Rg, Rt);
    mu = clamp(mu, -1.0, 1.0);
    muS = clamp(muS, -1.0, 1.0);
    float var = sqrt(1.0 - mu * mu) * sqrt(1.0 - muS * muS);
    nu = clamp(nu, muS * mu - var, muS * mu + var);

    float cthetamin = -sqrt(1.0 - (Rg / r) * (Rg / r));

    float3 v = float3(sqrt(1.0 - mu * mu), 0.0, mu);
    float sx = v.x == 0.0 ? 0.0 : (nu - muS * mu) / v.x;
    float3 s = float3(sx, sqrt(max(0.0, 1.0 - sx * sx - muS * muS)), muS);

    raymie = float3(0.0, 0.0, 0.0);

    // integral over 4.PI around x with two nested loops over w directions (theta,phi) -- Eq (7)
	[loop]
    for (uint itheta = 0; itheta < INSCATTER_SPHERICAL_INTEGRAL_SAMPLES; ++itheta) {
        float theta = (float(itheta) + 0.5) * dtheta;
        float ctheta = cos(theta);

        float greflectance = 0.0;
        float dground = 0.0;
        float3 gtransp = float3(0.0, 0.0, 0.0);
        if (ctheta < cthetamin) { // if ground visible in direction w
            // compute transparency gtransp between x and ground
            greflectance = float(AVERAGE_GROUND_REFLECTANCE) / float(M_PI);
            dground = -r * ctheta - sqrt(r * r * (ctheta * ctheta - 1.0) + Rg * Rg);
            gtransp = transmittance(trans, transSamp, Rg, -(r * ctheta + dground) / Rg, dground);
        }

		[loop]
        for (uint iphi = 0; iphi < 2 * INSCATTER_SPHERICAL_INTEGRAL_SAMPLES; ++iphi) {
            float phi = (float(iphi) + 0.5) * dphi;
            float dw = dtheta * dphi * sin(theta);
            float3 w = float3(cos(phi) * sin(theta), sin(phi) * sin(theta), ctheta);

            float nu1 = dot(s, w);
            float nu2 = dot(v, w);
            float pr2 = phaseFunctionR(nu2);
            float pm2 = phaseFunctionM(nu2);

            // compute irradiance received at ground in direction w (if ground visible) =deltaE
            float3 gnormal = (float3(0.0, 0.0, r) + dground * w) / Rg;
            float3 girradiance = irradiance(deltaE, deltaESamp, Rg, dot(gnormal, s));

            float3 raymie1; // light arriving at x from direction w

            // first term = light reflected from the ground and attenuated before reaching x, =T.alpha/PI.deltaE
            raymie1 = greflectance * girradiance * gtransp;

            // second term = inscattered light, =deltaS
			[branch]
            if (first == 1.0) {
                // first iteration is special because Rayleigh and Mie were stored separately,
                // without the phase functions factors; they must be reintroduced here
                float pr1 = phaseFunctionR(nu1);
                float pm1 = phaseFunctionM(nu1);
                float3 ray1 = texture4D(deltaSR, deltaSRSamp, r, w.z, muS, nu1).rgb;
                float3 mie1 = texture4D(deltaSM, deltaSMSamp, r, w.z, muS, nu1).rgb;
                raymie1 += ray1 * pr1 + mie1 * pm1;
            } else {
                raymie1 += texture4D(deltaSR, deltaSRSamp, r, w.z, muS, nu1).rgb;
            }

            // light coming from direction w and scattered in direction v
            // = light arriving at x from direction w (raymie1) * SUM(scattering coefficient * phaseFunction)
            // see Eq (7)
            raymie += raymie1 * (betaR * exp(-(r - Rg) / HR) * pr2 + betaMSca * exp(-(r - Rg) / HM) * pm2) * dw;
        }
    }

    // output raymie = J[T.alpha/PI.deltaE + deltaS] (line 7 in algorithm 4.1)
}

float4 PS(GS_OUT o) : SV_TARGET0
{
	float3 raymie;
    float mu, muS, nu;
    getMuMuSNu(float2(o.Tex.x * RES_MU_S * RES_NU, o.Tex.y * RES_MU), r, dhdH, mu, muS, nu);
    inscatter(r, mu, muS, nu, raymie);
    return float4(raymie, 1.0);
}

technique InscatterS
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