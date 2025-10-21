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

// Propiedades de la luz con camara
uniform float3 LightPosition;
uniform float3 EyePosition;

// Propiedades de luz
uniform float3 AmbientColor;
uniform float3 DiffuseColor;
uniform float3 SpecularColor;
uniform float KAmbient;
uniform float KDiffuse;
uniform float KSpecular;
uniform float Shininess;

// Entrada al vertex shader
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
};

// Salida del vertex shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;
    float3x3 TBN : TEXCOORD3;
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

    // Normal expresada en espacio mundo
    output.Normal = mul(input.Normal, InverseTransposeWorld);

    // Generar TBN
    float3 worldNormal = normalize(output.Normal.xyz);
    float3 tangent = float3(1, 0, 0);
    float3 bitangent = normalize(cross(worldNormal, tangent));
    tangent = normalize(cross(bitangent, worldNormal));
    output.TBN = float3x3(tangent, bitangent, worldNormal);

    return output;
}

// Fragment Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 L = normalize(LightPosition - input.WorldPosition.xyz);
    float3 V = normalize(EyePosition - input.WorldPosition.xyz);
    float3 H = normalize(L + V);

    float3 N = normalize(input.Normal.xyz);

    float NdotL = saturate(dot(N, L));
    float3 diffuse = KDiffuse * DiffuseColor * NdotL;

    float NdotH = saturate(dot(N, H));
    float3 specular = KSpecular * SpecularColor * pow(NdotH, Shininess);

    float3 ambient = KAmbient * AmbientColor;

    return float4(ambient + diffuse + specular, 1.0f);
}

technique Default
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};