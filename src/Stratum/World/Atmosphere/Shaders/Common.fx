#include "Constants.fx"

static const float AVERAGE_GROUND_REFLECTANCE = 0.1;

// Rayleigh
static const float HR = 8.0;
static const float3 betaR = float3(5.8e-3, 1.35e-2, 3.31e-2);
//static const float3 betaR = float3(2.9e-3, 0.65e-2, 1.67e-2);

// Mie
// default
static const float HM = 1.2;
static const float3 betaMSca = float3(4e-3, 4e-3, 4e-3);
static const float3 betaMEx = float3(4e-3, 4e-3, 4e-3) / 0.9; //betaMSca / 0.9;
static const float mieG = 0.8;
// clear sky
/*float HM = 1.2;
float3 betaMSca = float3(20e-3);
float3 betaMEx = betaMSca / 0.9;
float mieG = 0.76;*/
// partly cloudy
/*float HM = 3.0;
float3 betaMSca = float3(3e-3);
float3 betaMEx = betaMSca / 0.9;
float mieG = 0.65;*/

// ----------------------------------------------------------------------------
// NUMERICAL INTEGRATION PARAMETERS\n\
// ----------------------------------------------------------------------------

static const int TRANSMITTANCE_INTEGRAL_SAMPLES = 500;
static const int INSCATTER_INTEGRAL_SAMPLES = 50;
static const int IRRADIANCE_INTEGRAL_SAMPLES = 32;
static const int INSCATTER_SPHERICAL_INTEGRAL_SAMPLES = 16;

static const float M_PI = 3.141592657;

float CorrectMod(float a, float b)
{
	if( a >= 0.0 )
	{
		return fmod(a,b);
	}
	else
	{
		float result = fmod(-a,b);
		if(result == 0.0)
			return result;
		else
			return b - result;
	}
}

float2 getTransmittanceUV(float r, float mu)
{
	float ur, uMu;
	ur = sqrt((r - Rg) / (Rt - Rg));
	uMu = atan((mu + 0.15) / (1.0 + 0.15) * tan(1.5)) / 1.5;
	return float2(uMu, ur);
}

void getTransmittanceRMu(float2 tex, out float r, out float muS)
{
	r = tex.y;
	muS = tex.x;
	r = Rg + (r * r) * (Rt - Rg);
	muS = -0.15 + tan(1.5 * muS) / tan(1.5) * (1.0 + 0.15);
}

float2 getIrradianceUV(float r, float muS)
{
	float uR = (r - Rg) / (Rt - Rg);
	float uMuS = (muS + 0.2) / (1.0 + 0.2);
	return float2(uMuS, uR);
}

void getIrradianceRMuS(float2 tex, out float r, out float muS)
{
	r = Rg + tex.y * (Rt - Rg);
	muS = -0.2 + tex.x * (1.0 + 0.2);
}

float4 texture4D(Texture3D table, SamplerState samp, float r, float mu, float muS, float nu)
{
	float H = sqrt(Rt * Rt - Rg * Rg);
	float rho = sqrt(r * r - Rg * Rg);

	float rmu = r * mu;
	float delta = rmu * rmu - r * r + Rg * Rg;
	float4 cst = rmu < 0.0 && delta > 0.0 ? float4(1.0, 0.0, 0.0, 0.5 - 0.5 / float(RES_MU)) : float4(-1.0, H * H, H, 0.5 + 0.5 / float(RES_MU));
	float uR = 0.5 / float(RES_R) + rho / H * (1.0 - 1.0 / float(RES_R));
	float uMu = cst.w + (rmu * cst.x + sqrt(delta + cst.y)) / (rho + cst.z) * (0.5 - 1.0 / float(RES_MU));
	// paper formula 
	//float uMuS = 0.5 / float(RES_MU_S) + max((1.0 - exp(-3.0 * muS - 0.6)) / (1.0 - exp(-3.6)), 0.0) * (1.0 - 1.0 / float(RES_MU_S));
	// better formula
	float uMuS = 0.5 / float(RES_MU_S) + (atan(max(muS, -0.1975) * tan(1.26 * 1.1)) / 1.1 + (1.0 - 0.26)) * 0.5 * (1.0 - 1.0 / float(RES_MU_S));

	float lerp = (nu + 1.0) / 2.0 * (float(RES_NU) - 1.0);
	float uNu = floor(lerp);
	lerp = lerp - uNu;
	return table.Sample(samp, float3((uNu + uMuS) / float(RES_NU), uMu, uR)) * (1.0 - lerp) 
		+ table.Sample(samp, float3((uNu + uMuS + 1.0) / float(RES_NU), uMu, uR)) * lerp;
}

// potential source of error 1
void getMuMuSNu(float2 tex, float r, float4 dhdH, out float mu, out float muS, out float nu)
{
	// tex must be in space [0..w, 0..h] of the texture
	float x = tex.x - 0.5;
	float y = tex.y - 0.5;

	if (y < float(RES_MU) / 2.0)
	{
		float d = 1.0 - y / (float(RES_MU) / 2.0 - 1.0);
		d = min(max(dhdH.z, d * dhdH.w), dhdH.w * 0.999);
		mu = (Rg * Rg - r * r - d * d) / (2.0 * r * d);
		mu = min(mu, -sqrt(1.0 - (Rg / r) * (Rg / r)) - 0.001);
	}
	else
	{
		float d = (y - float(RES_MU) / 2.0) / (float(RES_MU) / 2.0 - 1.0);
		d = min(max(dhdH.x, d * dhdH.y), dhdH.y * 0.999);
		mu = (Rt * Rt - r * r - d * d) / (2.0 * r * d);
	}

	muS = CorrectMod(x, float(RES_MU_S)) / (float(RES_MU_S) - 1.0);
	// paper formula
    //muS = -(0.6 + log(1.0 - muS * (1.0 -  exp(-3.6)))) / 3.0;
    // better formula
    muS = tan((2.0 * muS - 1.0 + 0.26) * 1.1) / tan(1.26 * 1.1);
    nu = -1.0 + floor(x / float(RES_MU_S)) / (float(RES_NU) - 1.0) * 2.0;
}

// ----------------------------------------------------------------------------
// UTILITY FUNCTIONS\n\
// ----------------------------------------------------------------------------

// nearest intersection of ray r,mu with ground or top atmosphere boundary
// mu=cos(ray zenith angle at ray origin)
float limit(float r, float mu) 
{
    float dout = -r * mu + sqrt(r * r * (mu * mu - 1.0) + RL * RL);
    float delta2 = r * r * (mu * mu - 1.0) + Rg * Rg;
    if (delta2 >= 0.0) 
	{
        float din = -r * mu - sqrt(delta2);
        if (din >= 0.0) 
		{
            dout = min(dout, din);
        }
    }
    return dout;
}

// transmittance(=transparency) of atmosphere for infinite ray (r,mu)
// (mu=cos(view zenith angle)), intersections with ground ignored
float3 transmittance(Texture2D transmittanceTex, SamplerState transSamp, float r, float mu) 
{
	float2 uv = getTransmittanceUV(r, mu);
    return transmittanceTex.Sample(transSamp, uv).rgb;
}

// transmittance(=transparency) of atmosphere for infinite ray (r,mu)
// (mu=cos(view zenith angle)), or zero if ray intersects ground
float3 transmittanceWithShadow(Texture2D transmittanceTex, SamplerState transSamp, float r, float mu) 
{
    return mu < -sqrt(1.0 - (Rg / r) * (Rg / r)) ? float3(0.0, 0.0, 0.0) : transmittance(transmittanceTex, transSamp, r, mu);
}

// transmittance(=transparency) of atmosphere between x and x0
// assume segment x,x0 not intersecting ground
// r=||x||, mu=cos(zenith angle of [x,x0) ray at x), v=unit direction vector of [x,x0) ray
float3 transmittance(Texture2D transTex, SamplerState transSamp, float r, float mu, float3 v, float3 x0) 
{
    float3 result;
    float r1 = length(x0);
    float mu1 = dot(x0, v) / r;
    if (mu > 0.0) {
        result = min(transmittance(transTex, transSamp, r, mu) / transmittance(transTex, transSamp, r1, mu1), 1.0);
    } else {
        result = min(transmittance(transTex, transSamp, r1, -mu1) / transmittance(transTex, transSamp, r, -mu), 1.0);
    }
    return result;
}

// optical depth for ray (r,mu) of length d, using analytic formula
// (mu=cos(view zenith angle)), intersections with ground ignored
// H=height scale of exponential density function
float opticalDepth(float H, float r, float mu, float d) 
{
    float a = sqrt((0.5 / H) * r);
    float2 a01 = a * float2(mu, mu + d / r);
    float2 a01s = sign(a01);
    float2 a01sq = a01 * a01;
    float x = a01s.y > a01s.x ? exp(a01sq.x) : 0.0;
    float2 y = a01s / (2.3193 * abs(a01) + sqrt(1.52 * a01sq + 4.0)) * float2(1.0, exp(-d / H *(d / (2.0 * r) + mu)));
    return sqrt((6.2831 * H) * r) * exp((Rg - r) / H) * (x + dot(y, float2(1.0, -1.0)));
}

// transmittance(=transparency) of atmosphere for ray (r,mu) of length d
// (mu=cos(view zenith angle)), intersections with ground ignored
// uses analytic formula instead of transmittance texture
float3 analyticTransmittance(float r, float mu, float d) 
{
    return exp(-betaR * opticalDepth(HR, r, mu, d) - betaMEx * opticalDepth(HM, r, mu, d));
}

// transmittance(=transparency) of atmosphere between x and x0
// assume segment x,x0 not intersecting ground
// d = distance between x and x0, mu=cos(zenith angle of [x,x0) ray at x)
float3 transmittance(Texture2D transTex, SamplerState transSamp, float r, float mu, float d) 
{
    float3 result;
    float r1 = sqrt(r * r + d * d + 2.0 * r * mu * d);
    float mu1 = (r * mu + d) / r1;
    if (mu > 0.0) {
        result = min(transmittance(transTex, transSamp, r, mu) / transmittance(transTex, transSamp, r1, mu1), 1.0);
    } else {
        result = min(transmittance(transTex, transSamp, r1, -mu1) / transmittance(transTex, transSamp, r, -mu), 1.0);
    }
    return result;
}

float3 irradiance(Texture2D tex, SamplerState samp, float r, float muS) 
{
    float2 uv = getIrradianceUV(r, muS);
    return tex.Sample(samp, uv).rgb;
}

// Rayleigh phase function
float phaseFunctionR(float mu) 
{
    return (3.0 / (16.0 * M_PI)) * (1.0 + mu * mu);
}

// Mie phase function
float phaseFunctionM(float mu) 
{
	return 1.5 * 1.0 / (4.0 * M_PI) * (1.0 - mieG * mieG) * pow(1.0 + (mieG * mieG) - 2.0 * mieG * mu, -3.0 / 2.0) * (1.0 + mu * mu) / (2.0 + mieG * mieG);
}

// approximated single Mie scattering (cf. approximate Cm in paragraph 'Angular precision')
float3 getMie(float4 rayMie) 
{ // rayMie.rgb=C*, rayMie.w=Cm,r
	return rayMie.rgb * rayMie.w / max(rayMie.r, 1e-4) * (betaR.r / betaR);
}