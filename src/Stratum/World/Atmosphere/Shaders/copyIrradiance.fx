#include "Common.fx"

float k;

Texture2D deltaE;
SamplerState deltaESamp;

float4 PS(float2 tex : TEXCOORD0) : SV_TARGET0
{
	float2 uv = tex;
    return k * deltaE.Sample(deltaESamp, uv); // k=0 for line 4, k=1 for line 10
}

technique CopyIrradiance
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