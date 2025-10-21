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

// Propiedades del environment map
uniform float3 EyePosition;

// Textura base
texture BaseTexture;
sampler2D BaseTextureSampler = sampler_state
{
    Texture = (BaseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Textura del environment map
texture EnvironmentMap;
samplerCUBE EnvironmentMapSampler = sampler_state
{
    Texture = (EnvironmentMap);
    MagFilter = Linear;
    MinFilter = Linear;
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
    float4 Normal : TEXCOORD2;
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
    
    // Propagamos las coordenadas de textura
    output.TextureCoordinate = input.TextureCoordinate;
    
    // Normal expresada en espacion mundo
    output.Normal = mul(float4(normalize(input.Position.xyz), 1.0), InverseTransposeWorld);
	
    return output;
}

// Fragment Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Normalizamos
    float3 normal = normalize(input.Normal.xyz);
    float3 view = normalize(EyePosition.xyz - input.WorldPosition.xyz);
    
	// Obtener el texel de la textura base
    float3 baseColor = tex2D(BaseTextureSampler, input.TextureCoordinate).rgb;
    
	// Obtener texel del environment map
    float3 reflection = reflect(view, normal);
    float3 reflectionColor = texCUBE(EnvironmentMapSampler, reflection).rgb;
    float reflectionStrength = 0.5;

    // Coeficiente fresnel (mas reflejo con angulos rasantes)
    float fresnel = saturate((1.0 - dot(normal, view)));

    return float4(lerp(baseColor, reflectionColor, fresnel * reflectionStrength), 1);
}

technique Default
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};