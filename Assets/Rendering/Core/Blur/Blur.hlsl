#pragma once
#include "UnityCG.cginc"
#define PI 3.14159265

sampler2D _MainTex;
float4 _MainTex_TexelSize;

float Radius;
float Factor;

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

float4 SampleColor(const float2 uv)
{
    const float2 uvCorrected = uv - 2 * (uv - saturate(uv));
    const float4 color = tex2D(_MainTex, uvCorrected);
    return saturate(color);
}

float BlurGaussian(const int distance)
{
    const float normalizationFactor = 1.0f / (2.0f * (Radius * Radius / PI));
    const float distanceFactor = exp(-(distance * distance) / pow(Radius / PI, 2) / 2);
    return normalizationFactor * distanceFactor;
}

float4 BlurBox(const float2 uv, const float2 direction)
{
    float3 totalColor = tex2D(_MainTex, uv).xyz;
    float totalWeight = 1;

    for (int i = 1; i <= Radius; ++i)
    {
        totalWeight += 2;

        const float2 offset = i * _MainTex_TexelSize.xy * direction;
        totalColor += SampleColor(uv + offset);
        totalColor += SampleColor(uv - offset);
    }

    totalColor /= totalWeight;
    return float4(totalColor, 1.0f);
}

float4 BlurGaussian(const float2 uv, const float2 direction)
{
    float4 totalColor = 0;
    float totalWeight = 0.0f;

    const float centerWeight = BlurGaussian(0);
    totalWeight += centerWeight;

    const float4 centerColor = tex2D(_MainTex, uv);
    totalColor += centerColor * centerWeight;

    for (int i = 1; i <= Radius; ++i)
    {
        const float weight = BlurGaussian(i);
        totalWeight += weight * 2.0f;

        const float2 offset = i * _MainTex_TexelSize.xy * direction;
        totalColor += SampleColor(uv + offset) * weight;
        totalColor += SampleColor(uv - offset) * weight;
    }

    totalColor /= totalWeight;
    
    const float4 finalColor = lerp(centerColor, totalColor, Factor);
    
    return finalColor;
}

float4 Frag(const float2 uv, const float2 direction)
{
    #if ALGORITHM_BOX
    return BlurBox(uv, direction);
    #elif ALGORITHM_GAUSSIAN
    return BlurGaussian(uv, direction);
    #endif
}

float4 FragHorizontal(const v2f input) : SV_Target
{
    return Frag(input.uv, float2(1, 0));
}

float4 FragVertical(const v2f input) : SV_Target
{
    return Frag(input.uv, float2(0, 1));
}
