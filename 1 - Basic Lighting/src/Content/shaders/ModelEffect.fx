#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix WorldViewProjection;
float3 LightDirection;

texture DiffuseTexture;
sampler diffuseSampler = sampler_state
{
    Texture = (DiffuseTexture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord	: TEXCOORD0;
	float3 Normal 	: NORMAL0;
	float3 Binormal	: BINORMAL0;
	float3 Tangent	: TANGENT0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float LightAmount : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	output.Position = mul(input.Position, WorldViewProjection);
	output.TexCoord = input.TexCoord;
	
	// Calculate the World Normal
	float3 Normal = normalize(mul(input.Normal, World));

	// Apply the Light Amount as the Dot Product of Normal and Light Direction.
	output.LightAmount = saturate(dot(Normal, LightDirection));

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	// Look up the Diffuse Colour from the Diffuse Sampler
	float3 DiffuseColor = tex2D(diffuseSampler, input.TexCoord).rgb;

	// Factor the Colour by the Light Amount
	float4 Color = float4(DiffuseColor * input.LightAmount, 1);

	return Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};