//JumpFlood https://bgolus.medium.com/the-quest-for-very-wide-outlines-ba82ed442cd9
Shader "Dreambox/Outline"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #define NULL -1.0
        #define FLOAT_INFINITY ((float)(1e1000))
        ENDHLSL

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
                float4 textureSample = UNITY_SAMPLE_TEX2D(_BaseMap, input.uv);
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

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

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

            float2 Frag(Varyings input) : SV_Target
            {
                float2 position = input.positionCS.xy;
                uint mask = _BlitTexture.Load(int3(position, 0));
                return mask > 0 ? position : NULL;
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

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
            };

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

            float2 Frag(Varyings input) : SV_Target
            {
                float2 position = input.positionCS.xy;

                float bestDistance = FLOAT_INFINITY;
                float2 bestPosition;

                UNITY_UNROLL
                for (int u = -1; u <= 1; u++)
                {
                    UNITY_UNROLL
                    for (int v = -1; v <= 1; v++)
                    {
                        int2 offset = int2(u, v) * StepWidth;
                        int2 positionWithOffset = clamp(position + offset, 0, _BlitTexture_TexelSize.zw - 1);
                        float2 targetPosition = _BlitTexture.Load(int3(positionWithOffset, 0)).rg;

                        if (targetPosition.x == NULL)
                        {
                            continue;
                        }
                        
                        float2 disp = position - targetPosition;
                        float distance = dot(disp, disp);

                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestPosition = targetPosition;
                        }
                    }
                }

                return bestPosition;
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

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

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
                int Width;
                float Softness;
            };

            StructuredBuffer<OutlineVariant> VariantsBuffer;

            Texture2D _BlitTexture;
            float4 _BlitTexture_TexelSize;
            
            Texture2D<uint> OutlineMaskTexture;
            SamplerState sampler_OutlineMaskTexture;
             
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
                float2 position = input.positionCS.xy;

                uint mask = OutlineMaskTexture.Load(int3(position, 0)).r;
                if (mask > 0)
                {
                    return 0;
                }

                float2 targetPosition = _BlitTexture.Load(int3(position, 0)).rg;
                uint targetMask = OutlineMaskTexture.Load(int3(targetPosition, 0)).r;

                OutlineVariant variant = VariantsBuffer[targetMask - 1];
                float distance = length(targetPosition - position);
                
                float distanceFactor = 1 - distance / variant.Width;

                clip(distanceFactor);

                float4 outlineColor = variant.Color;
                float weight = pow(distanceFactor, variant.Softness);
                outlineColor.a *= weight;

                return outlineColor;
            }
            ENDHLSL
        }
    }
    Fallback Off
}