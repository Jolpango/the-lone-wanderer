#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
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
    float2 TexCoord : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 lightPosition = (input.TexCoord * 2.f) - 1.f;
    float lightDistance = length(lightPosition);
    float intensity = 1.f - lightDistance;
    float lightRadius = 1.f;

    float attenuation = 1.0 - (lightDistance / lightRadius);
    attenuation = clamp(attenuation, 0.0, 1.0);
    attenuation = pow(attenuation, 1.5);
    
    float4 color = tex2D(SpriteTextureSampler, input.TexCoord) * input.Color;
    return float4(color.rgb * attenuation, color.a);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};