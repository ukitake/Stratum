#include "Common.fx"

Texture2D transTex;
SamplerState transSamp;

float4 PS(float2 tex : TEXCOORD0) : SV_TARGET0
{
	float r, muS;
	getIrradianceRMuS(tex, r, muS);
	return float4(transmittance(transTex, transSamp, r, muS) * max(muS, 0.0), 0.0);
}

technique Irradiance1
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