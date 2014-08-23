cbuffer PerObject
{
	float4x4 World;
	float4x4 View;
	float4x4 ViewNoT;
	float4x4 Proj;
	float4x4 ViewProj;

	float GlobalTime;
	float3 CameraPosition;
	float3 CameraPositionHigh;
	float3 CameraPositionLow;
	float2 ViewportSize;
};