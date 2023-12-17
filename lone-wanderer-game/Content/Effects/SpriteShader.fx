#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 ambient_intensity;

texture SpriteTexture;
sampler SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord: TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float intensity = ambient_intensity.a;
    float4 ambient = float4(ambient_intensity.rgb * intensity, 1.f);
    float4 diffues = tex2D(SpriteTextureSampler, input.TexCoord) * input.Color;
    
    return diffues * ambient;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};