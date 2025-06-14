Shader "Hidden/Dreambox/Desaturation"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment FragDesaturation

            float Factor;
            
            float4 FragDesaturation(Varyings input) : SV_Target
            {
                float3 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb;
                float luminance = Luminance(color);
                float4 finalColor = float4(lerp(color, luminance, Factor), 1);
                return finalColor;
            }
            ENDHLSL
        }
    }
}