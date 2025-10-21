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

// Entrada al vertex shader
struct VertexShaderInput
{
    float4 Position : POSITION0;
};

// Salida del vertex shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 ScreenSpacePosition : TEXCOORD1;
};

// Vertex Shader
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Restablezco el output
    VertexShaderOutput output = (VertexShaderOutput) 0;
    
    // Multiplico matrices: Local → Mundo
    float4 worldPosition = mul(input.Position, World);
    
    // Multiplico matrices: Mundo → Vista
    float4 viewPosition = mul(worldPosition, View);
    
    // Multiplico matrices: Vista → Proyeccion
    output.Position = mul(viewPosition, Projection);
    output.ScreenSpacePosition = mul(viewPosition, Projection);
    
    return output;
}

// Fragment Shader
float4 MainPS(in VertexShaderOutput input) : COLOR
{
    float depth = input.ScreenSpacePosition.z / input.ScreenSpacePosition.w;
    return float4(depth, depth, depth, 1.0);
}

technique Default
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};