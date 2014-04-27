Texture2D CopyTexture;
SamplerState CopyTextureSamp;

float4 PS(float2 tex : TEXCOORD0) : SV_TARGET0
{
	return CopyTexture.Sample(CopyTextureSamp, tex);
}

technique TextureToTarget
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