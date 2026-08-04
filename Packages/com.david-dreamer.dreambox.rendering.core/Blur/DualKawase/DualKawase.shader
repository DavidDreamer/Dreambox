Shader "Hidden/Dreambox/PostProcessing/Blur/DualKawase"
{
    SubShader
    {
        Cull Off
		ZWrite Off
		ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureXR.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        uniform float2 _Offset;
        ENDHLSL

        Pass
        {
            Name "Downsample"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                
                float4 result = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv) * 4.0;

                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x, _Offset.y));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x, _Offset.y));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x, -_Offset.y));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x, -_Offset.y));

                result /= 8.0;
            
                return result;
            }
            ENDHLSL
        }

         Pass
        {
            Name "Upsample"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                
                float4 result = 0;

                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x * 2.0, 0));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x * 2.0, 0));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0, _Offset.y * 2.0));
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0, -_Offset.y * 2.0));

                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x, _Offset.y)) * 2.0;
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x, _Offset.y)) * 2.0;
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_Offset.x, -_Offset.y)) * 2.0;
                result += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-_Offset.x, -_Offset.y)) * 2.0;

                result /= 12.0;
            
                return result;
            }
            ENDHLSL
        }
    }
}