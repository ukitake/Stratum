#include "Common.fx"

static const float EPSILON_ATMOSPHERE = 0.002;
static const float EPSILON_INSCATTER = 0.004;
static const float EXPOSURE = 1.5;

struct VS_IN
{
	float3 pos : POSITION;
	float3 texAndCornerIndex : TEXCOORD;
};

struct VS_OUT
{
	float4 pos : SV_Position;
	float2 tex : TEXCOORD0;
	float3 farfrustumCornerViewSpace : TEXCOORD1; // view space translation, but rotated to world space
};

// textures for reading from gbuffer

Texture2D gbDepthTexture;
SamplerState gbDepthSampler;

Texture2D gbNormalTexture;
SamplerState gbNormalSampler;

Texture2D gbAlbedoTexture;
SamplerState gbAlbedoSampler;

Texture2D texTransmittance;
Texture2D texIrradiance;
Texture3D texInscatter;

SamplerState LinearSamp;

cbuffer Other
{
	float3 sunVector;
	float sunIntensity = 22.0;
};

cbuffer Camera
{
	float3 wPosCamera;
	float near_plane;
	float far_plane;
	float fov;
	float4 farfrustumCornersVS[4];
	float4x4 IView;
};

// input - d: view ray in world space
// output - offset: distance to atmosphere or 0 if within atmosphere
// output - maxPathLength: distance traversed within atmosphere
// output - return value: intersection occurred true/false
bool intersectAtmosphere(in float3 d, out float offset, out float maxPathLength)
{
	offset = 0.0f;
	maxPathLength = 0.0f;

	// vector from ray origin to center of the sphere
	float3 l = -wPosCamera; 
	float l2 = dot(l, l);
	float s = dot(l, d);

	// adjust top atmosphere boundary by small epsilon to prevent artifacts
	float r = Rt - EPSILON_ATMOSPHERE;
	float r2 = r*r;

	if(l2 <= r2)
	{
		// ray origin inside sphere, hit is ensured
		float m2 = l2 - (s * s);
		float q = sqrt(r2 - m2);
		maxPathLength = s + q;
		return true;
	}
	else if(s >= 0)
	{
		// ray starts outside in front of sphere, hit is possible
		float m2 = l2 - (s * s);
		if(m2 <= r2)
		{
			// ray hits atmosphere definitely
			float q = sqrt(r2 - m2);
			offset = s - q;
			maxPathLength = (s + q) - offset;
			return true;
		}
	}
	return false;
}

// input - surfacePos: reconstructed position of current pixel
// input - viewDir: view ray in world space
// input/output - attenuation: extinction factor along view path
// input/output - irradianceFactor: surface hit within atmosphere 1.0f 
// otherwise 0.0f
// output - return value: total in-scattered light
float3 GetInscatteredLight(in float3 surfacePos, in float3 viewDir, in out float3 attenuation, in out float irradianceFactor)
{
	float3 inscatteredLight = float3(0.0, 0.0, 0.0);
	float offset;
	float maxPathLength;

	if (intersectAtmosphere(viewDir, offset, maxPathLength))
	{
		float pathLength = distance(wPosCamera, surfacePos);
		// check if object occludes atmosphere
		if(pathLength > offset)
		{
			// offsetting camera
			float3 startPos = wPosCamera + offset * viewDir;
			float startPosHeight = length(startPos);
			pathLength -= offset;

			// starting position of path is now ensured to be inside atmosphere
			// was either originally there or has been moved to top boundary
			float muStartPos = dot(startPos, viewDir) / startPosHeight;
			float nuStartPos = dot(viewDir, sunVector);
			float musStartPos = dot(startPos, sunVector) / startPosHeight;

			// in-scattering for infinite ray (light in-scattered when 
			// no surface hit or object behind atmosphere)
			float4 inscatter = max(texture4D(texInscatter, LinearSamp, startPosHeight, muStartPos, musStartPos, nuStartPos), 0.0);
			float surfacePosHeight = length(surfacePos);
			float musEndPos = dot(surfacePos, sunVector) / surfacePosHeight;
			// check if object hit is inside atmosphere
			if(pathLength < maxPathLength)
			{
				// reduce total in-scattered light when surface hit 
				// within atmosphere
				// fíx described in chapter 5.1.1
				attenuation = analyticTransmittance(startPosHeight, muStartPos, pathLength);
				float muEndPos = dot(surfacePos, viewDir) / surfacePosHeight;
				float4 inscatterSurface = texture4D(texInscatter, LinearSamp, surfacePosHeight, muEndPos, musEndPos, nuStartPos);
					inscatter = max(inscatter - attenuation.rgbr * inscatterSurface, 0.0);
				irradianceFactor = 1.0;
			}
			else
			{
				// retrieve extinction factor for inifinte ray
				// fíx described in chapter 5.1.1
				attenuation = analyticTransmittance(startPosHeight, muStartPos, pathLength);
			}

			// avoids imprecision problems near horizon by interpolating between 
			// two points above and below horizon
			// fíx described in chapter 5.1.2
			float muHorizon = -sqrt(1.0 - (Rg / startPosHeight) * (Rg / startPosHeight));
			if (abs(muStartPos - muHorizon) < EPSILON_INSCATTER) 
			{
				float mu = muHorizon - EPSILON_INSCATTER;
				float samplePosHeight = sqrt(startPosHeight * startPosHeight + pathLength * pathLength + 2.0 * startPosHeight * pathLength * mu);
				float muSamplePos = (startPosHeight * mu + pathLength) / samplePosHeight;
				float4 inScatter0 = texture4D(texInscatter, LinearSamp, startPosHeight, mu, musStartPos, nuStartPos);
				float4 inScatter1 = texture4D(texInscatter, LinearSamp, samplePosHeight, muSamplePos, musEndPos, nuStartPos);
				float4 inScatterA = max(inScatter0 - attenuation.rgbr * inScatter1, 0.0);
				mu = muHorizon + EPSILON_INSCATTER;
				samplePosHeight = sqrt(startPosHeight * startPosHeight + pathLength * pathLength + 2.0 * startPosHeight * pathLength * mu);
				muSamplePos = (startPosHeight * mu + pathLength) / samplePosHeight;
				inScatter0 = texture4D(texInscatter, LinearSamp, startPosHeight, mu, musStartPos, nuStartPos);
				inScatter1 = texture4D(texInscatter, LinearSamp, samplePosHeight, muSamplePos, musEndPos, nuStartPos);
				float4 inScatterB = max(inScatter0 - attenuation.rgbr * inScatter1, 0.0);
				float t = ((muStartPos - muHorizon) + EPSILON_INSCATTER) / (2.0 * EPSILON_INSCATTER);
				inscatter = lerp(inScatterA, inScatterB, t);
			}
			// avoids imprecision problems in Mie scattering when sun is below
			//horizon
			// fíx described in chapter 5.1.3
			inscatter.w *= smoothstep(0.00, 0.02, musStartPos);
			float phaseR = phaseFunctionR(nuStartPos);
			float phaseM = phaseFunctionM(nuStartPos);
			inscatteredLight = max(inscatter.rgb * phaseR + getMie(inscatter) * phaseM, 0.0f);
			inscatteredLight *= sunIntensity;
		}
	}
	return inscatteredLight;
}

// input - surfacePos: reconstructed position of current pixel
// input - texC: texture coordinates
// input - attenuation: extinction factor along view path
// input - irradianceFactor: surface hit within atmosphere 1.0f 
// otherwise 0.0f
// output - return value: total reflected light + direct sunlight
float3 GetReflectedLight(in float3 surfacePos, in float2 texC, in float3 attenuation, in float irradianceFactor)
{
	// read contents of GBuffer
	float4 normalData = gbNormalTexture.Sample(gbNormalSampler, texC);
	float3 surfaceColor = gbAlbedoTexture.Sample(gbAlbedoSampler, texC).rgb;

	// decode normal and determine intensity of refected light at 
	// surface postiion
	float3 normal = normalData.xyz; 
	float lightIntensity = sunIntensity * 0.1 / 3.14159;
	float lightScale = max(dot(normal, sunVector), 0.0);
	
	// irradiance at surface position due to sky light
	float surfacePosHeight = length(surfacePos);
	float musSurfacePos = dot(surfacePos, sunVector) / surfacePosHeight;
	float3 irradianceSurface = irradiance(texIrradiance, LinearSamp, surfacePosHeight, musSurfacePos) * irradianceFactor;
	
	// attenuate direct sun light on its path from top of atmosphere to 
	// surface position
	float3 attenuationSunLight = transmittance(texTransmittance, LinearSamp, surfacePosHeight, musSurfacePos);
	float3 reflectedLight = surfaceColor * (lightScale * attenuationSunLight + irradianceSurface) * lightIntensity;
	
	// attenuate again on path from surface position to camera
	reflectedLight *= attenuation;
	return reflectedLight;
}

VS_OUT VS(VS_IN input)
{
	VS_OUT output;
	output.pos = float4(input.pos, 1);
	output.tex = input.texAndCornerIndex.xy;
	float3 farCorner = farfrustumCornersVS[input.texAndCornerIndex.z];
	output.farfrustumCornerViewSpace = mul(float4(farCorner, 0), IView);
	return output;
}

float3 HDR(float3 color)
{
	return 1.0f - exp(-EXPOSURE * color);
}

float4 PS(VS_OUT input) : SV_Target0
{
	float depth = gbDepthTexture.Sample(gbDepthSampler, input.tex);
	float3 cameraToFar = input.farfrustumCornerViewSpace;  // camera is at origin

	// multiply view space depth by a ray that passes through this pixel on 
	// its way to the far clip plane
	float3 viewdir = normalize(cameraToFar);
	float3 surfacePos = wPosCamera + cameraToFar * depth;  //wPosCamera + cameraToNear + depth * nearToFar;

	// obtaining the view direction vector
	float3 attenuation = float3(1.0, 1.0, 1.0);
	float irradianceFactor = 0.0;
	float3 inscatteredLight = GetInscatteredLight(surfacePos, viewdir, attenuation, irradianceFactor);
	float3 reflectedLight = GetReflectedLight(surfacePos, input.tex, attenuation, irradianceFactor);
	float3 col = HDR(reflectedLight + inscatteredLight);
	return float4(col, saturate(exp(col.r) * 0.4 + length(col.gb))); // wrong
}

technique Atmosphere
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		ComputeShader = null;
		PixelShader = PS;
	}
}