Shader "Hidden/Dreambox/PostProcessing/Blur"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureXR.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        uniform int _Radius;
        uniform Buffer<float> _GaussianWeights;

        float4 BlurGaussian(float2 uv, float2 direction)
        {
	        float4 result = 0;
	        for (int i = -_Radius; i <= _Radius; i++)
	        {
		        float2 offset = uv + direction * i * _BlitTexture_TexelSize.xy;
		        float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, offset);
                float weight = _GaussianWeights[i + _Radius];
		        result += color * weight;
	        }

	        return result;
        }

        float4 Frag(float2 uv, float2 direction)
        {
	        return BlurGaussian(uv, direction);
        }
        ENDHLSL

        Pass
        {
            Name "Horizontal"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
	            return Frag(input.texcoord, float2(1, 0));
            }
            ENDHLSL
        }

        Pass
        {
            Name "Vertical"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
	            return Frag(input.texcoord, float2(0, 1));
            }
            ENDHLSL
        }
    }
}