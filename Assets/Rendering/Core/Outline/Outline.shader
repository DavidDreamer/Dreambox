//JumpFlood https://bgolus.medium.com/the-quest-for-very-wide-outlines-ba82ed442cd9
Shader "Dreambox/Outline"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTex", 2D) = "clear"
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "Mask"

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "UnityCG.cginc"

            UNITY_DECLARE_TEX2D(_BaseMap);
            uniform uint Variant;

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                return output;
            }

            uint Frag(Varyings input) : SV_Target
            {
                const float4 textureSample = UNITY_SAMPLE_TEX2D(_BaseMap, input.uv);
                clip(textureSample.a - 1);
                return Variant;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Init"

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
            };

            Texture2D<uint> _BlitTexture;
            SamplerState sampler_BlitTexture;

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

                output.positionCS = pos;
                output.texcoord   = uv;

                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                const float2 position = input.positionCS.xy;
                const uint configIndex = _BlitTexture.Load(int3(position, 0));
                const float4 dataPacked = float4(position, configIndex, 0);
                return dataPacked;
            }
            ENDHLSL
        }

        Pass
        {
            Name "JumpFlood"

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
            };

            #define FLOAT_INFINITY ((float)(1e1000))

            Texture2D _BlitTexture;
            float4 _BlitTexture_TexelSize;

            int StepWidth;

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

                output.positionCS = pos;
                output.texcoord   = uv;

                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                const float2 position = input.positionCS.xy;

                float minDistance = FLOAT_INFINITY;
                float2 finalPosition;
                float outputVariantIndex = 0;

                UNITY_UNROLL
                for (int u = -1; u <= 1; u++)
                {
                    UNITY_UNROLL
                    for (int v = -1; v <= 1; v++)
                    {
                        const int2 offset = int2(u, v) * StepWidth;
                        const int2 positionWithOffset = clamp(position + offset, 0, _BlitTexture_TexelSize.zw - 1);
                        const float3 sample = _BlitTexture.Load(int3(positionWithOffset, 0)).rgb;
                        const float2 targetPosition = sample.rg;
                        const float variantIndex = sample.b;

                        if (variantIndex == 0)
                        {
                            continue;
                        }
                        
                        const float2 disp = position - targetPosition;
                        const float distance = dot(disp, disp);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            outputVariantIndex = variantIndex;
                            finalPosition = targetPosition;
                        }
                    }
                }

                return float4(finalPosition, outputVariantIndex, 0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Decode"

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define UNITY_PI 3.14159265359f

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
            };

            struct OutlineVariant
            {
                float4 Color;
                float Width;
                float Softness;
                float4 FillColor;
                float4 FillFlickColor;
                float FillFlickRate;
            };

            StructuredBuffer<OutlineVariant> VariantsBuffer;

            Texture2D _BlitTexture;
            float4 _BlitTexture_TexelSize;

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

                output.positionCS = pos;
                output.texcoord   = uv;

                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                const float2 position = input.positionCS.xy;
                const float3 sample = _BlitTexture.Load(int3(position, 0)).rgb;
                const float index = sample.b;

                if (index == 0)
                {
                    return 0;
                }

                const OutlineVariant variant = VariantsBuffer[index - 1];
                const float distance = length(sample.rg - position);
                const float width = variant.Width * _BlitTexture_TexelSize.w;
                const float weight = pow(1 - saturate(distance / width), variant.Softness);
                float4 outlineColor = variant.Color;
                outlineColor.a *= weight;
                
                // Inner filling edge need +1.5 inset for good anti-aliasing
                const float fill_weight = saturate(1.5 - distance);
                
                const float4 fill_color = lerp(variant.FillColor, variant.FillFlickColor,
                                               (cos(_Time.y * variant.FillFlickRate - UNITY_PI) + 1) / 2);

                float4 final_color = lerp(outlineColor, fill_color, fill_weight);

                return final_color;
            }
            ENDHLSL
        }
    }
    Fallback Off
}