//JumpFlood https://bgolus.medium.com/the-quest-for-very-wide-outlines-ba82ed442cd9
Shader "Hidden/Dreambox/Outline"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTex", 2D) = "clear"
    }

    HLSLINCLUDE
    #define FLOAT_INFINITY ((float)(1e1000))

    #pragma target 4.5
    
    #include "UnityCG.cginc"

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

            sampler2D _BaseMap;
            uint VariantIndex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f data;
                data.vertex = UnityObjectToClipPos(v.vertex);
                data.uv = v.uv;
                return data;
            }

            uint frag(v2f input) : SV_Target
            {
                const float alpha = tex2D(_BaseMap, input.uv).a;
                clip(alpha - 1);
                return VariantIndex;
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

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                const float2 position = i.pos;
                const uint configIndex = _MainTex.Load(int3(position, 0));
                const float4 dataPacked = float4(position, configIndex, 0);
                return dataPacked;
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

            Texture2D _MainTex;
            float4 _MainTex_TexelSize;

            int StepWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                const float2 position = i.pos;

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
                        const int2 positionWithOffset = clamp(position + offset, 0, _MainTex_TexelSize.zw - 1);
                        const float3 sample = _MainTex.Load(int3(positionWithOffset, 0));
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

            Texture2D _MainTex;
            float4 _MainTex_TexelSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                const float2 position = i.pos;
                const float3 sample = _MainTex.Load(int3(position, 0));
                const float index = sample.b;
                const OutlineVariant variant = VariantsBuffer[index - 1];
                const float distance = length(sample.rg - position);
                const float width = variant.Width * _MainTex_TexelSize.w;
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