#include "Common.fx"

float opticalDepth(float H, float r, float mu) 
{
    float result = 0.0;
    float dx = limit(r, mu) / float(TRANSMITTANCE_INTEGRAL_SAMPLES);
    float xi = 0.0;
    float yi = exp(-(r - Rg) / H);
    for (int i = 1; i <= TRANSMITTANCE_INTEGRAL_SAMPLES; ++i) {
        float xj = float(i) * dx;
        float yj = exp(-(sqrt(r * r + xj * xj + 2.0 * xj * r * mu) - Rg) / H);
        result += (yi + yj) / 2.0 * dx;
        xi = xj;
        yi = yj;
    }
    return mu < -sqrt(1.0 - (Rg / r) * (Rg / r)) ? 1e9 : result;
}

float4 PS(float2 tex : TEXCOORD0) : SV_TARGET0
{
	float r, muS;
	getTransmittanceRMu(tex, r, muS);
	float3 depth = betaR * opticalDepth(HR, r, muS) + betaMEx * opticalDepth(HM, r, muS);
	return float4(exp(-depth), 0);
}

technique Transmittance
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