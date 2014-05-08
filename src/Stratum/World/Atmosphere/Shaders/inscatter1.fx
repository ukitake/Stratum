static const float Rg = 6360.0;
static const float Rt = 6420.0;
static const float RL = 6421.0;

static const float TRANSMITTANCE_W = 256.0;
static const float TRANSMITTANCE_H = 64.0;

static const float SKY_W = 64.0;
static const float SKY_H = 16.0;

static const float RES_NU = 8.0;
static const float RES_MU_S = 32.0;
static const float RES_MU = 128.0;
static const float RES_R = 32.0;

//numerical integration parameters:
#define TRANSMITTANCE_INTEGRAL_SAMPLES 500
#define INSCATTER_INTEGRAL_SAMPLES 50
#define IRRADIANCE_INTEGRAL_SAMPLES 32
#define INSCATTER_SPHERICAL_INTEGRAL_SAMPLES 16
static const float M_PI = 3.141592657;

static const float AVERAGE_GROUND_REFLECTANCE = 0.1;

// Rayleigh
static const float HR = 8.0;
static const float3 betaR = float3(5.8e-3, 1.35e-2, 3.31e-2);

// Mie
// default
static const float HM = 1.2;
static const float3 betaMSca = float3(4e-3, 4e-3, 4e-3);
static const float3 betaMEx = float3(4e-3, 4e-3, 4e-3) / 0.9; //betaMSca / 0.9;
static const float mieG = 0.8;

float r;
float4 dhdH;
int layer;

Texture2D _transTex;
SamplerState _transSamp;

// nearest intersection of ray r,mu with ground or top atmosphere boundary
// mu=cos(ray zenith angle at ray origin)
float limit( in float r, in float mu ) {
        float dout = -r*mu + sqrt( r*r*(mu*mu - 1.0) + RL*RL);
        float delta2 = r*r*(mu*mu - 1.0) + Rg*Rg;

		[branch]
        if( delta2 >= 0.0 ) {
                float din = -r*mu - sqrt(delta2);
                
				[branch]
				if( din >= 0.0 )               dout = min( dout, din );
        }
        return dout;
}

void getMuMuSNu( in float r, in float4 dhdH, in float2 TexPos, out float mu, out float muS, out float nu ) {
        float x = TexPos.x - 0.5;
        float y = TexPos.y - 0.5;

		[branch]
		if( y < RES_MU/2.0 ) {
                float d = 1.0 - y/(RES_MU/2.0 - 1.0);
                d = min( max( dhdH.z, d*dhdH.w), dhdH.w*0.999 );
                mu = (Rg*Rg - r*r - d*d)/(2.0*r*d);
                mu = min( mu, -sqrt(1.0 - (Rg/r)/(Rg/r)) - 0.001);
        }
        else {
                float d = (y - RES_MU/2.0)/(RES_MU/2.0 - 1.0);
                d = min( max( dhdH.x, d*dhdH.y ), dhdH.y*0.999 );
                mu = (Rt*Rt - r*r - d*d)/(2.0*r*d);
        }
        muS = (x - RES_MU_S*floor(x/RES_MU_S))/(RES_MU_S - 1.0);
        // paper formula
    //muS = -(0.6 + log(1.0 - muS * (1.0 -  exp(-3.6)))) / 3.0;
    // better formula
        muS = tan( (2.0*muS - 1.0 + 0.26)*1.1 )/tan( 1.26*1.1 );
        nu = -1.0 + floor(x/RES_MU_S)/(RES_NU - 1.0)*2.0;
}

float2 getTransmittanceUV( in float r, in float mu ) {
        float uR, uMu;
        uR = sqrt( (r - Rg)/(Rt - Rg) );
        uMu = atan( (mu + 0.15)/1.15*tan(1.5) )/1.5;
        return float2( uMu, uR );
}

// transmittance(=transparency) of atmosphere for infinite ray (r,mu)
// (mu=cos(view zenith angle)), intersections with ground ignored
float3 transmittance( Texture2D TEX, SamplerState SAMPLER, in float r, in float mu ) {
        float2 uv = getTransmittanceUV( r, mu );
        return TEX.SampleLevel( SAMPLER, uv, 0 ).xyz;
}

// transmittance(=transparency) of atmosphere between x and x0
// assume segment x,x0 not intersecting ground
// d = distance between x and x0, mu=cos(zenith angle of [x,x0) ray at x)
float3 transmittance( Texture2D TEX, SamplerState SAMPLER,  in float r, in float mu, in float d ) {
        float3 result;
        float r1 = sqrt( r*r + d*d + 2.0*r*mu*d );
        float mu1 = (r*mu + d)/r1;

		[branch]
        if( mu > 0.0 )         result = min( transmittance( TEX, SAMPLER, r, mu )/transmittance( TEX, SAMPLER, r1, mu1 ), 1.0 );
        else                            result = min( transmittance( TEX, SAMPLER, r1, -mu1 )/transmittance( TEX, SAMPLER, r, -mu ), 1.0 );
        return result;
}

struct VS_OUT 
{
	float4 position : SV_POSITION;
};

VS_OUT VS(float3 pos : POSITION)
{
	VS_OUT o;
	o.position = float4(pos, 1);
	return o;
}

struct GS_OUT 
{
	float4 PosH : SV_POSITION;
	float2 Tex : TEXCOORD0;
	uint RTIndex : SV_RenderTargetArrayIndex;
};

[maxvertexcount(3)]
void GS(triangle VS_OUT In[3], inout TriangleStream<GS_OUT> triStream)
{
	GS_OUT o;
	
	for (uint i = 0; i < 3; i++)
	{
		o.PosH = In[i].position;
		o.Tex = In[i].position.xy * 0.5 + float2(0.5, 0.5);
		o.Tex.y = 1 - o.Tex.y;
		o.RTIndex = layer;
		triStream.Append(o);
	}
}

struct PS_OUT {
        float4 deltaSR  : SV_Target0;
        float4 deltaSM  : SV_Target1;
};

void integrand( in float r, in float mu, in float muS, in float nu, in float t, out float3 ray, out float3 mie ) {
        ray = float3( 0.0, 0.0, 0.0 );
        mie = float3( 0.0, 0.0, 0.0 );
        float ri = sqrt(r*r + t*t + 2.0*r*mu*t);
        float muSi = (nu*t + muS*r)/ri;
        ri = max( Rg, ri );

		[branch]
        if( muSi >= -sqrt( 1.0 - Rg*Rg/(ri*ri) ) ) {
                float3 ti = transmittance( _transTex, _transSamp, r, mu, t )*transmittance( _transTex, _transSamp, ri, muSi );
                ray = exp( -(ri - Rg)/HR )*ti;
                mie = exp( -(ri - Rg)/HM )*ti;
        }
}

void inscatter( in float r, in float mu, in float muS, in float nu, out float3 ray, out float3 mie ) {
        ray = float3( 0.0, 0.0, 0.0 );
        mie = float3( 0.0, 0.0, 0.0 );
        float dx = limit( r, mu )/ float(INSCATTER_INTEGRAL_SAMPLES);
        float xi = 0.0;
        float3 rayi;
        float3 miei;
        integrand( r, mu, muS, nu, 0.0, rayi, miei );

		[loop]
        for( uint i = 1; i <= INSCATTER_INTEGRAL_SAMPLES; ++i ) {
                float xj = float(i)*dx;
                float3 rayj;
                float3 miej;
                integrand( r, mu, muS, nu, xj, rayj, miej );
                ray += (rayi + rayj)/2.0*dx;
                mie += (miei + miej)/2.0*dx;
                xi = xj;
                rayi = rayj;
                miei = miej;
        }
        ray *= betaR;
        mie *= betaMSca;
}

PS_OUT PS(GS_OUT o) 
{
        float3 ray;
        float3 mie;
        float mu, muS, nu;
        getMuMuSNu( r, dhdH, float2(o.Tex.x * RES_MU_S * RES_NU, o.Tex.y * RES_MU), mu, muS, nu );
        inscatter( r, mu, muS, nu, ray, mie );

        PS_OUT Out;
        Out.deltaSR = float4( ray, 0.0 );
        Out.deltaSM = float4( mie, 0.0f );
        return Out;
}

technique Inscatter1
{
	pass Pass0
	{
		Profile = 11.0;
		VertexShader = VS;
		HullShader = null;
		DomainShader = null;
		GeometryShader = GS;
		ComputeShader = null;
		PixelShader = PS;
	}
}