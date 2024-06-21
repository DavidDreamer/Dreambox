//JumpFlood https://bgolus.medium.com/the-quest-for-very-wide-outlines-ba82ed442cd9
Shader "Hidden/Dreambox/Outline"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTex", 2D) = "clear"
    }

    HLSLINCLUDE
    #define FLOAT_INFINITY ((float)(1e1000))
    #define SOBEL_THRESHOLD 0.005

    #pragma target 4.5
    #pragma multi_compile_instancing

    #include "UnityCG.cginc"

    struct OutlineVariant
    {
        float4 OutlineColor;
        float4 FillColor;
        float Width;
        float Softness;
        float SoftnessPower;
        float PixelOffset;
        float4 FillFlickColor;
        float FillFlickRate;
        float CutOffWidth;
    };

    StructuredBuffer<OutlineVariant> VariantsBuffer;

    uint PackToR14G14B4(const uint3 rgb)
    {
        const uint r = rgb.x << 18 & 0xFFFC0000;
        const uint g = rgb.y << 4 & 0x3FFF0;
        const uint b = rgb.z & 0xF;
        return r | g | b;
    }

    uint3 UnpackFromR14G14B4(const uint rgb)
    {
        const uint r = rgb >> 18 & 0x3FFF;
        const uint g = rgb >> 4 & 0x3FFF;
        const uint b = rgb & 0xF;
        return uint3(r, g, b);
    }

    float3x3 Sample3x3(const Texture2D<uint> source, const float4 texelSize, const int2 uv)
    {
        float3x3 result;
        UNITY_UNROLL
        for (int u = 0; u < 3; u++)
        {
            UNITY_UNROLL
            for (int v = 0; v < 3; v++)
            {
                const int2 offset = int2(u - 1, v - 1);
                const uint2 positionWithOffset = clamp(uv + offset, 0, texelSize.zw - 1);
                const uint calculatedSample = source.Load(int3(positionWithOffset, 0));
                if (calculatedSample == 0)
                {
                    result[u][v] = 0;
                }
                else
                {
                    result[u][v] = 1;
                }
            }
        }

        return result;
    }
    ENDHLSL
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "Mask"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            uint ConfigIndex;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            float4 vert(appdata v) : SV_POSITION
            {
                const float4 pos = UnityObjectToClipPos(v.vertex);
                return pos;
            }

            uint frag() : SV_Target
            {
                return ConfigIndex;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Init"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            Texture2D<uint> _MainTex;
            float4 _MainTex_TexelSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            uint frag(v2f i) : SV_Target
            {
                const uint2 position = i.pos.xy;
                const uint configIndex = _MainTex.Load(int3(position, 0));
                const float3x3 samples = Sample3x3(_MainTex, _MainTex_TexelSize, position);
                const float mainSample = samples._m11;

                if (mainSample > 0.99)
                    return PackToR14G14B4(uint3(position, configIndex));

                if (mainSample < 0.01)
                    return 0;

                const float2 sobel = -float2(
                    samples[0][0] + samples[0][1] * 2.0 + samples[0][2] - samples[2][0] - samples[2][1] * 2.0 - samples[
                        2][2],
                    samples[0][0] + samples[1][0] * 2.0 + samples[2][0] - samples[0][2] - samples[1][2] * 2.0 - samples[
                        2][2]
                );

                if (abs(sobel.x) <= SOBEL_THRESHOLD && abs(sobel.y) <= SOBEL_THRESHOLD)
                    return PackToR14G14B4(uint3(position, configIndex));

                const float sobelNormalized = normalize(sobel);
                const float2 offset = sobelNormalized * (1.0 - mainSample);
                const float2 uvWithOffset = position + offset;
                return PackToR14G14B4(uint3(uvWithOffset, configIndex));
            }
            ENDHLSL
        }

        Pass
        {
            Name "JumpFlood"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            Texture2D<uint> _MainTex;
            float4 _MainTex_TexelSize;

            int StepWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            uint frag(v2f i) : SV_Target
            {
                const uint2 position = i.pos.xy;

                float minDistance = FLOAT_INFINITY;
                float2 finalPosition;
                float finalIndex;

                UNITY_UNROLL
                for (int u = -1; u <= 1; u++)
                {
                    UNITY_UNROLL
                    for (int v = -1; v <= 1; v++)
                    {
                        const int2 offset = int2(u, v) * StepWidth;
                        const int2 positionWithOffset = clamp(position + offset, 0, _MainTex_TexelSize.zw - 1);
                        const float3 calculatedSample = UnpackFromR14G14B4(_MainTex.Load(int3(positionWithOffset, 0)));
                        const float2 targetPosition = calculatedSample.rg;

                        const float2 disp = position - targetPosition;
                        const uint configIndex = calculatedSample.b - 1;
                        const float cutOffWidth = VariantsBuffer[configIndex].CutOffWidth * _MainTex_TexelSize.w;
                        const float distanceSqr = dot(disp, disp) - cutOffWidth * cutOffWidth;

                        if (calculatedSample.b != 0 && distanceSqr < minDistance)
                        {
                            minDistance = distanceSqr;
                            finalIndex = calculatedSample.b;
                            finalPosition = targetPosition;
                        }
                    }
                }

                if (isinf(minDistance))
                {
                    return 0;
                }

                return PackToR14G14B4(uint3(finalPosition, finalIndex));
            }
            ENDHLSL
        }

        Pass
        {
            Name "Decode"

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            Texture2D<uint> _MainTex;
            float4 _MainTex_TexelSize;

            float UnscaledTime;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                const uint2 position = i.pos.xy;
                const float3 calculatedSample = UnpackFromR14G14B4(_MainTex.Load(int3(position, 0)));

                if (calculatedSample.b == 0)
                    return 0;

                const uint config_index = calculatedSample.b - 1;
                const OutlineVariant variant = VariantsBuffer[config_index];
                const float distance = length(calculatedSample.rg - position) - variant.PixelOffset;

                const float width = variant.Width * _MainTex_TexelSize.w;

                const float4 adjusted_outline_color = variant.OutlineColor * saturate(width);

                // Calculate outline mask
                // +1.0 is because encoded nearest position is float a pixel inset
                // Not +0.5 because we want the anti-aliased edge to be aligned between pixels
                // Distance is already in pixels, so this is already perfectly anti-aliased!
                const float outline_weight = saturate(width + 1.0 - distance);

                // Inner filling edge need +1.5 inset for good anti-aliasing
                const float fill_weight = saturate(1.5 - distance);

                const float outline_fade = 1 - saturate((distance - 1.0 - width * (1 - variant.Softness)) / width);

                // Adjust outline alfa to proper anti-aliasing and softness
                const float4 outline_color = adjusted_outline_color * float4(
                    1, 1, 1, outline_weight * pow(outline_fade, variant.SoftnessPower));

                // Blend fill with flickering
                const float4 fill_color = lerp(variant.FillColor, variant.FillFlickColor,
                                               (cos(UnscaledTime * variant.FillFlickRate - UNITY_PI) + 1) / 2);

                // Blend between outline and fill
                float4 final_color = lerp(outline_color, fill_color, fill_weight);

                return final_color;
            }
            ENDHLSL
        }
    }
    Fallback Off
}