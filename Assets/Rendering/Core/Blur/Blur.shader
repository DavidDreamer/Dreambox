Shader "Hidden/Dreambox/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Blur.hlsl"
        ENDHLSL

        Pass
        {
            Name "Horizontal"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment FragHorizontal
            #pragma multi_compile_local_fragment ALGORITHM_BOX ALGORITHM_GAUSSIAN
            #pragma multi_compile_local_fragment WRAP_MODE_CLAMP WRAP_MODE_MIRROR
            ENDHLSL
        }

        Pass
        {
            Name "Vertical"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment FragVertical
            #pragma multi_compile_local_fragment ALGORITHM_BOX ALGORITHM_GAUSSIAN
            #pragma multi_compile_local_fragment WRAP_MODE_CLAMP WRAP_MODE_MIRROR
            ENDHLSL
        }
    }
}