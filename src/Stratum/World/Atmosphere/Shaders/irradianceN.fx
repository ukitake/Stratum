#include "Common.fx"

Texture3D deltaSR;
SamplerState deltaSRSamp;

Texture3D deltaSM;
SamplerState deltaSMSamp;

float first;
static const float dphi = M_PI / float(IRRADIANCE_INTEGRAL_SAMPLES);
static const float dtheta = M_PI / float(IRRADIANCE_INTEGRAL_SAMPLES);

float4 PS(float2 tex : TEXCOORD0) : SV_TARGET0
{
	float r, muS;
	getIrradianceRMuS(tex, r, muS);
	float3 s = float3(sqrt(max(1.0 - muS * muS, 0.0)), 0.0, muS);
	float3 result = float3(0.0, 0.0, 0.0);

	// integral over 2.PI around x with two nested loops over w directions (theta,phi) -- Eq (15)
	[loop]
    for (int iphi = 0; iphi < 2 * IRRADIANCE_INTEGRAL_SAMPLES; ++iphi) {
        float phi = (float(iphi) + 0.5) * dphi;
        
		[loop]
		for (int itheta = 0; itheta < IRRADIANCE_INTEGRAL_SAMPLES / 2; ++itheta) {
            float theta = (float(itheta) + 0.5) * dtheta;
            float dw = dtheta * dphi * sin(theta);
            float3 w = float3(cos(phi) * sin(theta), sin(phi) * sin(theta), cos(theta));
            float nu = dot(s, w);

			[branch]
            if (first == 1.0) {
                // first iteration is special because Rayleigh and Mie were stored separately,
                // without the phase functions factors; they must be reintroduced here
                float pr1 = phaseFunctionR(nu);
                float pm1 = phaseFunctionM(nu);
                float3 ray1 = texture4D(deltaSR, deltaSRSamp, r, w.z, muS, nu).rgb;
                float3 mie1 = texture4D(deltaSM, deltaSMSamp, r, w.z, muS, nu).rgb;
                result += (ray1 * pr1 + mie1 * pm1) * w.z * dw;
            } else {
                result += texture4D(deltaSR, deltaSRSamp, r, w.z, muS, nu).rgb * w.z * dw;
            }
        }
    }

    return float4(result, 1.0);
}

technique IrradianceN
{
	pass Pass0
	{
		Profile = 11.0;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		ComputeShader = null;
		PixelShader = PS;
	}
}