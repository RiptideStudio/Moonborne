sampler TextureSampler;
float2 LightPosition;
float3 LightColor;
float LightIntensity;
float LightRadius;

float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 fragPos = texCoord * 1920;
    float dist = distance(fragPos, LightPosition);
    
    float intensity = saturate(1 - (dist / LightRadius)) * LightIntensity;

    float4 color = tex2D(TextureSampler, texCoord);
    color.rgb *= LightColor * intensity;
    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
