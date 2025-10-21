#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Transformaciones
uniform float4x4 World;
uniform float4x4 View;
uniform float4x4 Projection;
uniform float4x4 InverseTransposeWorld;
uniform float Bias;

// Propiedades de environment map
uniform float2 ShadowMapSize;
uniform float3 LightPosition;
uniform float4x4 LightViewProjection;

// Tiling para texturas
uniform float2 Tiling;

// Constantes
static const float ModulatedEpsilon = 0.000041200182749889791011810302734375;
static const float MaxEpsilon = 0.000023200045689009130001068115234375;

// Textura base
texture BaseTexture;
sampler2D BaseTextureSampler = sampler_state
{
    Texture = (BaseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

// Textura del environment map
texture ShadowMap;
sampler2D ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Entrada al vertex shader
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD0;
};

// Salida del vertex shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinate : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 ProjectedLightPosition : TEXCOORD2;
    float4 Normal : TEXCOORD3;
};

// Vertex Shader
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Restablezco el output
    VertexShaderOutput output = (VertexShaderOutput) 0;
    
    // Multiplico matrices: Local → Mundo
    float4 worldPosition = mul(input.Position, World);
    output.WorldPosition = worldPosition;
    
    // Multiplico matrices: Mundo → Vista
    float4 viewPosition = mul(worldPosition, View);
    
    // Multiplico matrices: Vista → Proyeccion
    output.Position = mul(viewPosition, Projection);
    
    // Propagamos las coordenadas de textura escaladas
    output.TextureCoordinate = input.TextureCoordinate * Tiling;
    
    // Normal expresada en espacio mundo
    float3 worldNormal = normalize(mul(float4(input.Normal, 0), InverseTransposeWorld).xyz);
    output.Normal = float4(worldNormal, 0);
    
    // Posicion de la luz proyectada
    output.ProjectedLightPosition = mul(worldPosition, LightViewProjection);
    
    return output;
}

// Fragment Shader
float4 MainPS(in VertexShaderOutput input) : COLOR
{
    float3 lightPosProj = input.ProjectedLightPosition.xyz / input.ProjectedLightPosition.w;

    float2 shadowUV = 0.5 * lightPosProj.xy + float2(0.5, 0.5);
    shadowUV.y = 1.0f - shadowUV.y;

    float3 normal = normalize(input.Normal.xyz); 
    float3 lightDir = normalize(LightPosition - input.WorldPosition.xyz);

    float NdotL = max(dot(normal, lightDir), 0);
    float bias = Bias;
    float visibility = 0.0;
    float2 texelSize = 1.0 / ShadowMapSize;

    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            float depthFromSM = tex2D(ShadowMapSampler, shadowUV + float2(x, y) * texelSize).r + bias;
            visibility += step(lightPosProj.z, depthFromSM);
        }
    }
    visibility /= 9.0;

    float4 baseColor = tex2D(BaseTextureSampler, input.TextureCoordinate);

    float AmbientStrength = 0.65;
    float lighting = saturate(AmbientStrength + (1.0 - AmbientStrength) * NdotL);
    baseColor.rgb *= lighting;

    float ShadowDarkness = 0.8;
    baseColor.rgb *= lerp(ShadowDarkness, 1.0, visibility);

    return baseColor;
}

technique Default
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};