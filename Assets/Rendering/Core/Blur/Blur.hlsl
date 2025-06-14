#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

float Radius;
float Factor;

float4 SampleColor(float2 uv)
{
    #ifdef WRAP_MODE_MIRROR
    float2 uvCorrected = uv - 2 * (uv - saturate(uv));
    return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uvCorrected);
    #else
    float2 uvCorrected = saturate(uv);
    return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uvCorrected);
    #endif
}

float BlurGaussian(int distance)
{
    float normalizationFactor = 1.0f / (2.0f * (Radius * Radius / PI));
    float distanceFactor = exp(-(distance * distance) / pow(Radius / PI, 2) / 2);
    return normalizationFactor * distanceFactor;
}

float4 BlurBox(float2 uv, float2 direction)
{
    float3 totalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).xyz;
    float totalWeight = 1;

    for (int i = 1; i <= Radius; ++i)
    {
        totalWeight += 2;

        float2 offset = i * _BlitTexture_TexelSize.xy * direction;
        totalColor += SampleColor(uv + offset).rgb;
        totalColor += SampleColor(uv - offset).rgb;
    }

    totalColor /= totalWeight;
    return float4(totalColor, 1.0f);
}

float4 BlurGaussian(float2 uv, float2 direction)
{
    float4 totalColor = 0;
    float totalWeight = 0.0f;

    float centerWeight = BlurGaussian(0);
    totalWeight += centerWeight;

    float4 centerColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);
    totalColor += centerColor * centerWeight;

    for (int i = 1; i <= Radius; ++i)
    {
        float weight = BlurGaussian(i);
        totalWeight += weight * 2.0f;

        float2 offset = i * _BlitTexture_TexelSize.xy * direction;
        totalColor += SampleColor(uv + offset) * weight;
        totalColor += SampleColor(uv - offset) * weight;
    }

    totalColor /= totalWeight;
    
    float4 finalColor = lerp(centerColor, totalColor, Factor);
    
    return finalColor;
}

float4 Frag(float2 uv, float2 direction)
{
    #if ALGORITHM_BOX
    return BlurBox(uv, direction);
    #elif ALGORITHM_GAUSSIAN
    return BlurGaussian(uv, direction);
    #endif
}

float4 FragHorizontal(Varyings input) : SV_Target
{
    return Frag(input.texcoord, float2(1, 0));
}

float4 FragVertical(Varyings input) : SV_Target
{
    return Frag(input.texcoord, float2(0, 1));
}
