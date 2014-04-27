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

cbuffer PerObject 
{
	float4x4 World;
	float4x4 View;
	float4x4 ViewNoT;
	float4x4 Proj;

	// material parameters
};

cbuffer DirectionalLight 
{
	float3 dLightDirection;
	float4 dLightColor;
};

// textures for writing to gbuffer

Texture2D albedoTexture;
SamplerState albedoSampler;

Texture2D normalMap;
SamplerState normalSampler;

Texture2D displacementMap;
SamplerState displacementSampler;

// textures for reading from gbuffer

Texture2D gbDepthTexture;
SamplerState gbDepthSampler;

Texture2D gbNormalTexture;
SamplerState gbNormalSampler;

Texture2D gbAlbedoTexture;
SamplerState gbAlbedoSampler;

Texture2D elevationMap;
SamplerState elevationSampler;

struct HS_Constant 
{
	float edges[3] : SV_TessFactor;
	float inside : SV_InsideTessFactor;
};

/////////////////////////////////////////////////////
// POSITION
/////////////////////////////////////////////////////
struct VS_IN_POS
{
	float3 pos : POSITION;
};

struct VS_OUT_POS_TESS
{
	float3 wPos : POSITION;
};

struct VS_OUT_POS
{
	float4 pos : SV_POSITION;
	float3 vpos : TEXCOORD0;
};

VS_OUT_POS VS_POS(VS_IN_POS input) 
{
	VS_OUT_POS output;
	output.vpos = mul(mul(float4(input.pos, 1.0), World), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	return output;
}

VS_OUT_POS_TESS VS_POS_TESS(VS_IN_POS input)
{
	VS_OUT_POS_TESS output;
	output.wPos = mul(float4(input.pos, 1.0), World).xyz;
	return output;
}

HS_Constant HS_POS_CONST(InputPatch<VS_OUT_POS_TESS, 3> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_Constant output;

	float3 worldPatchCenter = 0.3333333 * (inputPatch[0].wPos + inputPatch[1].wPos + inputPatch[2].wPos);
	float eyeDistance = distance(worldPatchCenter, wPosCamera);

	float tessAmount = 64.0 * saturate((far_plane - eyeDistance) / (far_plane - near_plane));

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = tessAmount;
    output.edges[1] = tessAmount;
    output.edges[2] = tessAmount;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside = tessAmount;

    return output;
}

[domain("tri")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("HS_POS_CONST")]
VS_OUT_POS_TESS HS_POS(InputPatch<VS_OUT_POS_TESS, 3> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	// VS_OUT_POS_TESS is exactly the structure we need here... reuse it
	VS_OUT_POS_TESS output;
	output.wPos = inputPatch[pointId].wPos;
	return output;
}

[domain("tri")]
VS_OUT_POS DS_POS(HS_Constant input, float3 uvwCoord : SV_DomainLocation, const OutputPatch<VS_OUT_POS_TESS, 3> patch)
{
	VS_OUT_POS output;
	float3 wVertexPosition = uvwCoord.x * patch[0].wPos + uvwCoord.y * patch[1].wPos + uvwCoord.z * patch[2].wPos;
	output.vpos = mul(float4(wVertexPosition, 1.0), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	return output;
}

GBuffer PS_POS(VS_OUT_POS input)
{
	GBuffer output;
	
	// store linear depth: view-space z over far_plane distance
	output.Depth = input.vpos.z / far_plane;

	// no normal map or input normals
	float3 wvNormal = cross(ddx(input.vpos), ddy(input.vpos));
	wvNormal = normalize(wvNormal);
	output.Normal = float4(wvNormal, 1.0);

	// no texture or vertex colors
	output.Albedo = float4(1, 1, 1, 1);

	return output;
}

/////////////////////////////////////////////////////
// POSITION, TEXTURE
/////////////////////////////////////////////////////
struct VS_IN_POS_TEX
{
	float3 pos : POSITION;
	float2 tex : TEXCOORD;
};

struct VS_OUT_POS_TEX_TESS
{
	float3 wPos : POSITION;
	float2 tex : TEXCOORD;
};

struct VS_OUT_POS_TEX
{
	float4 pos : SV_POSITION;
	float3 vpos : TEXCOORD0;
	float2 tex : TEXCOORD1;
};

VS_OUT_POS_TEX VS_POS_TEX(VS_IN_POS_TEX input)
{
	VS_OUT_POS_TEX output;
	output.vpos = mul(mul(float4(input.pos, 1.0), World), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.tex = input.tex;
	return output;
}

VS_OUT_POS_TEX_TESS VS_POS_TEX_TESS(VS_IN_POS_TEX input)
{
	VS_OUT_POS_TEX_TESS output;
	output.wPos = mul(float4(input.pos, 1.0), World).xyz;
	output.tex = input.tex;
	return output;
}

HS_Constant HS_POS_TEX_CONST(InputPatch<VS_OUT_POS_TEX_TESS, 3> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_Constant output;

	float3 worldPatchCenter = 0.3333333 * (inputPatch[0].wPos + inputPatch[1].wPos + inputPatch[2].wPos);
	float eyeDistance = distance(worldPatchCenter, wPosCamera);

	float tessAmount = 64.0 * saturate((far_plane - eyeDistance) / (far_plane - near_plane));

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = tessAmount;
    output.edges[1] = tessAmount;
    output.edges[2] = tessAmount;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside = tessAmount;

    return output;
}

[domain("tri")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("HS_POS_TEX_CONST")]
VS_OUT_POS_TEX_TESS HS_POS_TEX(InputPatch<VS_OUT_POS_TEX_TESS, 3> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	// VS_OUT_POS_TEX_TESS is exactly the structure we need here... reuse it
	VS_OUT_POS_TEX_TESS output;
	output.wPos = inputPatch[pointId].wPos;
	output.tex = inputPatch[pointId].tex;
	return output;
}

[domain("tri")]
VS_OUT_POS_TEX DS_POS_TEX(HS_Constant input, float3 uvwCoord : SV_DomainLocation, const OutputPatch<VS_OUT_POS_TEX_TESS, 3> patch)
{
	VS_OUT_POS_TEX output;
	float3 wVertexPosition = uvwCoord.x * patch[0].wPos + uvwCoord.y * patch[1].wPos + uvwCoord.z * patch[2].wPos;
	float2 texCoord = uvwCoord.x * patch[0].tex + uvwCoord.y * patch[1].tex * uvwCoord.z * patch[2].tex;
	output.vpos = mul(float4(wVertexPosition, 1.0), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.tex = texCoord;
	return output;
}

GBuffer PS_POS_TEX(VS_OUT_POS_TEX input)
{
	GBuffer output;
	
	// store linear depth: view-space z over far_plane distance
	// negate because right-handed coordinate system... if left-handed negation not necessary
	output.Depth = input.vpos.z / far_plane;

	// no normal map or input normals
	float3 wvNormal = cross(ddy(input.vpos), ddx(input.vpos));
	wvNormal = normalize(wvNormal);
	output.Normal = float4(wvNormal, 1.0);

	// sample albedo texture
	output.Albedo = albedoTexture.Sample(albedoSampler, input.tex);

	return output;
}


/////////////////////////////////////////////////////
// POSITION COLOR
/////////////////////////////////////////////////////
struct VS_IN_POS_COL
{
	float3 pos : POSITION;
	float4 col : COLOR;
};

struct VS_OUT_POS_COL
{
	float4 pos : SV_POSITION;
	float3 vpos : TEXCOORD0;
	float4 col : COLOR0;
};

struct VS_OUT_POS_COL_TESS
{
	float3 wPos : POSITION;
	float4 col : COLOR;
};

VS_OUT_POS_COL VS_POS_COL(VS_IN_POS_COL input)
{
	VS_OUT_POS_COL output;
	output.vpos = mul(mul(float4(input.pos, 1.0), World), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.col = input.col;
	return output;
}

VS_OUT_POS_COL_TESS VS_POS_COL_TESS(VS_IN_POS_COL input)
{
	VS_OUT_POS_COL_TESS output;
	output.wPos = mul(float4(input.pos, 1.0), World).xyz;
	output.col = input.col;
	return output;
}

HS_Constant HS_POS_COL_CONST(InputPatch<VS_OUT_POS_COL_TESS, 3> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_Constant output;

	float3 worldPatchCenter = 0.3333333 * (inputPatch[0].wPos + inputPatch[1].wPos + inputPatch[2].wPos);
	float eyeDistance = distance(worldPatchCenter, wPosCamera);

	float tessAmount = 64.0 * saturate((far_plane - eyeDistance) / (far_plane - near_plane));

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = tessAmount;
    output.edges[1] = tessAmount;
    output.edges[2] = tessAmount;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside = tessAmount;

    return output;
}

[domain("tri")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("HS_POS_COL_CONST")]
VS_OUT_POS_COL_TESS HS_POS_COL(InputPatch<VS_OUT_POS_COL_TESS, 3> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	// VS_OUT_POS_COL_TESS is exactly the structure we need here... reuse it
	VS_OUT_POS_COL_TESS output;
	output.wPos = inputPatch[pointId].wPos;
	output.col = inputPatch[pointId].col;
	return output;
}

[domain("tri")]
VS_OUT_POS_COL DS_POS_COL(HS_Constant input, float3 uvwCoord : SV_DomainLocation, const OutputPatch<VS_OUT_POS_COL_TESS, 3> patch)
{
	VS_OUT_POS_COL output;
	float3 wVertexPosition = uvwCoord.x * patch[0].wPos + uvwCoord.y * patch[1].wPos + uvwCoord.z * patch[2].wPos;
	float4 color = uvwCoord.x * patch[0].col + uvwCoord.y * patch[1].col * uvwCoord.z * patch[2].col;
	output.vpos = mul(float4(wVertexPosition, 1.0), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.col = color;
	return output;
}

GBuffer PS_POS_COL(VS_OUT_POS_COL input)
{
	GBuffer output;
	
	// store linear depth: view-space z over far_plane distance
	// negate because right-handed coordinate system... if left-handed negation not necessary
	output.Depth = input.vpos.z / far_plane;

	// no normal map or input normals
	float3 wvNormal = cross(ddy(input.vpos), ddx(input.vpos));
	wvNormal = normalize(wvNormal);
	output.Normal = float4(wvNormal, 1.0);

	// interpolated vertex color
	output.Albedo = input.col;

	return output;
}

/////////////////////////////////////////////////////
// Clear
/////////////////////////////////////////////////////

VS_OUT_POS VS_Clear(VS_IN_POS input)
{
	// pass through vertices... they should already be in 
	// homogeneous clip space
	VS_OUT_POS output;
	output.pos = float4(input.pos, 1);
	output.vpos = float3(0, 0, 0);
	return output;
}

GBuffer PS_Clear(VS_OUT_POS input)
{
	GBuffer output;
	output.Depth = 1.0;
	output.Normal = float4(0, 0, 0, 0);
	output.Albedo = float4(0, 0, 0, 1);
	return output;
}

/////////////////////////////////////////////////////
// Directional Light
/////////////////////////////////////////////////////

struct VS_IN_DLight 
{
	float3 pos : POSITION;
	float3 texAndCornerIndex : TEXCOORD;
};

struct VS_OUT_DLight
{
	float4 pos : SV_Position;
	float2 tex : TEXCOORD0;
	float3 frustumCornerViewSpace : TEXCOORD1;
};

VS_OUT_DLight VS_DLight(VS_IN_DLight input)
{
	VS_OUT_DLight output;
	output.pos = float4(input.pos, 1);
	output.tex = input.texAndCornerIndex.xy;
	output.frustumCornerViewSpace = frustumCornersVS[input.texAndCornerIndex.z];
	return output;
}

float4 PS_DLight(VS_OUT_DLight input) : SV_Target0
{
	float4 normal = gbNormalTexture.Sample(gbNormalSampler, input.tex);
	float depth = gbDepthTexture.Sample(gbDepthSampler, input.tex);
	float4 albedo = gbAlbedoTexture.Sample(gbAlbedoSampler, input.tex);

	if (depth == 1.0){
		clip(-1);
	}

	// multiply view space depth by a ray that passes through this pixel on 
	// its way to the far clip plane
	// TODO THIS IS WRONG... frustum should not be in world space but it is
	float3 vsPosition = depth * far_plane * normalize(input.frustumCornerViewSpace);

	float3 color = saturate(dot(-dLightDirection, normal)) * albedo.rgb * (vsPosition / vsPosition);
	return float4(color, 1.0);
}

/////////////////////////////////////////////////////
// WebMercatorTerrain
/////////////////////////////////////////////////////

struct VS_IN_WMTERRAIN
{
	float3 pos : POSITION;
	float4 tex : TEXCOORD0;
	float4 texBounds : TEXCOORD1;
	float dataReady : TEXCOORD2;
};

struct VS_OUT_WMTERRAIN
{
	float3 pos : POSITION;
	float4 tex : TEXCOORD0;
	float4 texBounds : TEXCOORD1;
	float dataReady : TEXCOORD2;
};

struct PS_IN_WMTERRAIN 
{
	float4 pos : SV_POSITION;
	float3 wpos : TEXCOORD0;
	float3 vpos : TEXCOORD1;
	float4 tex : TEXCOORD2;
	float4 texBounds : TEXCOORD3;
	float dataReady : TEXCOORD4;
	float4 ppos : TEXCOORD5;
};

struct HS_CONSTANT_WMTERRAIN 
{
	float edges[4] : SV_TessFactor;
	float inside[2] : SV_InsideTessFactor;
};

static const float PI = 3.14159265f;

VS_OUT_WMTERRAIN VS_WMTERRAIN(VS_IN_WMTERRAIN input)
{
	VS_OUT_WMTERRAIN output;
	output.pos = input.pos;
	output.tex = input.tex;
	output.texBounds = input.texBounds;
	output.dataReady = input.dataReady;
	return output;
}

HS_CONSTANT_WMTERRAIN HS_CON_WMTERRAIN(InputPatch<VS_OUT_WMTERRAIN, 4> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_CONSTANT_WMTERRAIN output;

	float tessAmount = 64;

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = tessAmount;
    output.edges[1] = tessAmount;
    output.edges[2] = tessAmount;
	output.edges[3] = tessAmount;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside[0] = tessAmount;
    output.inside[1] = tessAmount;

    return output;
}

[domain("quad")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(4)]
[patchconstantfunc("HS_CON_WMTERRAIN")]
VS_OUT_WMTERRAIN HS_WMTERRAIN(InputPatch<VS_OUT_WMTERRAIN, 4> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	// VS_OUT_POS_TESS is exactly the structure we need here... reuse it
	VS_OUT_WMTERRAIN output;
	output.pos = inputPatch[pointId].pos;
	output.tex = inputPatch[pointId].tex;
	output.texBounds = inputPatch[pointId].texBounds;
	output.dataReady = inputPatch[pointId].dataReady;
	return output;
}

[domain("quad")]
PS_IN_WMTERRAIN DS_WMTERRAIN(HS_CONSTANT_WMTERRAIN input, float2 uvCoord : SV_DomainLocation, const OutputPatch<VS_OUT_WMTERRAIN, 4> patch)
{
	PS_IN_WMTERRAIN output;
	float3 topMidpoint = lerp(patch[0].pos, patch[1].pos, uvCoord.x);
	float3 bottomMidpoint = lerp(patch[3].pos, patch[2].pos, uvCoord.x);
	float3 vertexPosition = lerp(topMidpoint, bottomMidpoint, uvCoord.y);

	float2 topTex = lerp(patch[0].tex.xy, patch[1].tex.xy, uvCoord.x);
	float2 botTex = lerp(patch[3].tex.xy, patch[2].tex.xy, uvCoord.x);
	float2 texCoord = lerp(topTex, botTex, uvCoord.y);
	float lat = vertexPosition.y * (PI / 180.0);
	float lon = vertexPosition.x * (PI / 180.0);

	// lat long to cartesian
	float3 ret;
    ret.x = 6359.99 * cos(lat) * cos(lon);
	ret.y = 6359.99 * sin(lat);
	ret.z = 6359.99 * cos(lat) * sin(lon);
	double3 dret = double3(normalize(ret));
	dret = dret * 6359.99L;
	//float4 elevation = elevationMap.SampleLevel(elevationSampler, float2((vertexPosition.x + 180) / 360, 1 - (vertexPosition.y + 90) / 180), 0);
	//dret = dret * clamp((6359.99L + (double(lerp(0, 25.85L, elevation.x)))), 6359.99L, 7000.0L);

	dret = dret - double3(wPosCamera);
	
	double4x4 dview = double4x4(ViewNoT);
	double3 dvpos = mul(double4(dret, 1.0L), dview).xyz;

	ret = mul(float4(ret, 1.0), World).xyz;
	output.wpos = ret;
	output.vpos = float3(dvpos);
	//output.vpos = mul(float4(output.wpos, 1.0), View).xyz;
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.ppos = output.pos;
	//output.pos = float4(dspos);
	output.tex = float4(texCoord, patch[0].tex.z, patch[0].tex.w);
	output.texBounds = patch[0].texBounds;
	output.dataReady = patch[0].dataReady;
	return output;
}

Texture2DArray gpuTileCache;
SamplerState cacheSampler;

GBuffer PS_WMTERRAIN(PS_IN_WMTERRAIN input)
{
	GBuffer output;
	
	// store linear depth
	output.Depth = input.vpos.z / far_plane;
	
	// no normal map or input normals
	float3 wNormal = normalize(input.wpos);
	output.Normal = float4(wNormal, 1);

	float2 tx = float2(asin(input.wpos.y / 6359.99), atan2(input.wpos.z, input.wpos.x));
	float2 txDeg = tx * 180 / 3.14159265f;
	float2 texMerc, texCoords;

	if (input.tex.w > 11 && input.dataReady > 0)
	{
		texMerc = input.tex.xy;
		texCoords = input.tex.xy;
	}
	else
	{
		float lat = txDeg.x;
		float lon = txDeg.y;
		texMerc.x = lon * 20037508.34 / 180;
		texMerc.y = log(tan((90 + lat) * 3.14159265f / 360)) / (3.14159265f / 180);
		texMerc.y = texMerc.y * 20037508.34 / 180;

		float2 minBounds = input.texBounds.xy;
		float2 maxBounds = input.texBounds.zw;

		texCoords = float2((texMerc.x - minBounds.x) / (maxBounds.x - minBounds.x), 1 - (texMerc.y - minBounds.y) / (maxBounds.y - minBounds.y));
	}
	//return float4(input.tex.x, input.tex.y, input.tex.z, 1);
	//output.Albedo = float4(texCoords.x, texCoords.y, 0, 1);
	//output.Albedo = float4(input.ppos.z / input.ppos.w, 0, 0, 1);
	output.Albedo = gpuTileCache.Sample(cacheSampler, float3(texCoords, input.tex.z));
	return output;
}

technique WebMercatorTerrain
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_WMTERRAIN;
		HullShader = HS_WMTERRAIN;
		DomainShader = DS_WMTERRAIN;
		GeometryShader = null;
		PixelShader = PS_WMTERRAIN;
		ComputeShader = null;
	}
}

/////////////////////////////////////////////////////
// Terrain
/////////////////////////////////////////////////////

struct VS_IN_TERRAIN
{
	float3 pos : POSITION;
	float2 tex : TEXCOORD0;
};

struct VS_OUT_TERRAIN
{
	float3 pos : POSITION;
	float2 tex : TEXCOORD0;
};

struct PS_IN_TERRAIN 
{
	float4 pos : SV_POSITION;
	float3 wpos : TEXCOORD0;
	float3 vpos : TEXCOORD1;
	float2 tex : TEXCOORD2;
	float4 ppos : TEXCOORD3;
};

struct HS_CONSTANT_TERRAIN 
{
	float edges[4] : SV_TessFactor;
	float inside[2] : SV_InsideTessFactor;
};

VS_OUT_TERRAIN VS_TERRAIN(VS_IN_TERRAIN input)
{
	VS_OUT_TERRAIN output;
	output.pos = input.pos;
	output.tex = input.tex;
	return output;
}

HS_CONSTANT_TERRAIN HS_CON_TERRAIN(InputPatch<VS_OUT_TERRAIN, 4> inputPatch, uint patchId : SV_PrimitiveID)
{    
    HS_CONSTANT_TERRAIN output;

	float tessAmount = 64;

    // Set the tessellation factors for the three edges of the triangle.
    output.edges[0] = tessAmount;
    output.edges[1] = tessAmount;
    output.edges[2] = tessAmount;
	output.edges[3] = tessAmount;

    // Set the tessellation factor for tessallating inside the triangle.
    output.inside[0] = tessAmount;
    output.inside[1] = tessAmount;

    return output;
}

[domain("quad")]
[partitioning("fractional_even")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(4)]
[patchconstantfunc("HS_CON_TERRAIN")]
VS_OUT_TERRAIN HS_TERRAIN(InputPatch<VS_OUT_TERRAIN, 4> inputPatch, uint pointId : SV_OutputControlPointID, uint patchId : SV_PrimitiveID)
{
	// VS_OUT_POS_TESS is exactly the structure we need here... reuse it
	VS_OUT_TERRAIN output;
	output.pos = inputPatch[pointId].pos;
	output.tex = inputPatch[pointId].tex;
	return output;
}

[domain("quad")]
PS_IN_TERRAIN DS_TERRAIN(HS_CONSTANT_TERRAIN input, float2 uvCoord : SV_DomainLocation, const OutputPatch<VS_OUT_TERRAIN, 4> patch)
{
	PS_IN_TERRAIN output;
	float3 topMidpoint = lerp(patch[0].pos, patch[1].pos, uvCoord.x);
	float3 bottomMidpoint = lerp(patch[3].pos, patch[2].pos, uvCoord.x);
	float3 vertexPosition = lerp(topMidpoint, bottomMidpoint, uvCoord.y);

	float2 topTex = lerp(patch[0].tex.xy, patch[1].tex.xy, uvCoord.x);
	float2 botTex = lerp(patch[3].tex.xy, patch[2].tex.xy, uvCoord.x);
	float2 texCoord = lerp(topTex, botTex, uvCoord.y);

	double3 vertexPosD = normalize(double3(vertexPosition));
	vertexPosD = vertexPosD * 6360.0;
	double3 vertexPosDView = vertexPosD - double3(wPosCamera);

	double4x4 dview = double4x4(ViewNoT);
	double3 dvpos = mul(double4(vertexPosDView, 1.0L), dview).xyz;

	output.wpos = float3(vertexPosD);
	output.vpos = float3(dvpos);
	output.pos = mul(float4(output.vpos, 1.0), Proj);
	output.ppos = output.pos;
	output.tex = texCoord;
	return output;
}

GBuffer PS_TERRAIN(PS_IN_TERRAIN input)
{
	GBuffer output;
	
	// store linear depth
	output.Depth = input.vpos.z / far_plane;
	
	// no normal map or input normals
	float3 wNormal = normalize(input.wpos);
	output.Normal = float4(wNormal, 1);

	float2 texCoords = input.tex;

	output.Albedo = float4(input.tex, 0, 1);
	return output;
}

technique Terrain
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_TERRAIN;
		HullShader = HS_TERRAIN;
		DomainShader = DS_TERRAIN;
		GeometryShader = null;
		PixelShader = PS_TERRAIN;
		ComputeShader = null;
	}
}

technique Clear
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_Clear;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_Clear;
		ComputeShader = null;
	}
}

technique Position
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_POS;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_POS;
		ComputeShader = null;
	}
}

technique Position_Tess
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_POS_TESS;
		HullShader = HS_POS;
		DomainShader = DS_POS;
		GeometryShader = null;
		PixelShader = PS_POS;
		ComputeShader = null;
	}
}

technique PositionTexture
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_POS_TEX;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_POS_TEX;
		ComputeShader = null;
	}
}

technique PositionTexture_Tess
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_POS_TEX_TESS;
		HullShader = HS_POS_TEX;
		DomainShader = DS_POS_TEX;
		GeometryShader = null;
		PixelShader = PS_POS_TEX;
		ComputeShader = null;
	}
}

technique PositionColor
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_POS_COL;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_POS_COL;
		ComputeShader = null;
	}
}

technique PositionColor_Tess
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_POS_COL_TESS;
		HullShader = HS_POS_COL;
		DomainShader = DS_POS_COL;
		GeometryShader = null;
		PixelShader = PS_POS_COL;
		ComputeShader = null;
	}
}

technique DirectionalLight
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS_DLight;
		HullShader = null;
		DomainShader = null;
		GeometryShader = null;
		PixelShader = PS_DLight;
		ComputeShader = null;
	}
}

