Shader "Hidden/Dreambox/PostProcessing/Blur/Kawase"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "Kawase"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureXR.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            uniform float2 _Offset;

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                float4 result = 0;

                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x, _Offset.y));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x, _Offset.y));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x, -_Offset.y));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x, -_Offset.y));

                result /= 4.0;
            
                return result;
            }
            ENDHLSL
        }
    }
}