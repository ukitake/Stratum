
// MRT GBuffer structure
struct GBuffer 
{
	float Depth : SV_Target0;
	float3 Normal : SV_Target1;
	float4 Albedo : SV_Target2;
};

cbuffer Camera
{
	float3 wPosCamera;
	float near_plane;
	float far_plane;
	float fov;
	float4 frustumCornersVS[4];
};

cbuffer MaterialParamsPerObject
{
	// material parameters
};

cbuffer DirectionalLight 
{
	float3 dLightDirection;
	float4 dLightColor;
};

cbuffer PointLight
{
	float3 pLightPosition;
	float4 pLightColor;
};

// gbuffer textures

Texture2D gbDepthTexture;
SamplerState gbDepthSampler;

Texture2D gbNormalTexture;
SamplerState gbNormalSampler;

Texture2D gbAlbedoTexture;
SamplerState gbAlbedoSampler;

