Shader "Hidden/Dreambox/PostProcessing/Blur/Box"
{
    Properties
    {
        _KernelSize ("KernelSize", Integer) = 3
        _Radius ("Radius", Integer) = 1
        _Scale ("Scale", Float) = 1
    }

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

        uniform int _KernelSize;
        uniform int _Radius;
        uniform float _Scale;
  
        float4 Frag(float2 uv, float2 direction)
        {
            float4 result = 0;

	        for (int i = -_Radius; i <= _Radius; i++)
	        {
		        float2 offset = uv + direction * i * _Scale * _BlitTexture_TexelSize.xy;
		        float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, offset);
		        result += color;
	        }

            result /= _KernelSize;

	        return result;
        }
        ENDHLSL

        Pass
        {
            Name "NonSeparable"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                float4 result = 0;
                
                for (int i = -_Radius; i <= _Radius; i++)
                {
                    for (int j = -_Radius; j <= _Radius; j++)
                    {
                        float2 offset = uv + float2(i, j) * _Scale * _BlitTexture_TexelSize.xy;
                        float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, offset);
                        result += color;
                    }
                }

                result /= _KernelSize * _KernelSize;

                return result;
            }
            ENDHLSL
        }

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