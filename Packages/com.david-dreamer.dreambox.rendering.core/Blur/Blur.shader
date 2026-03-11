Shader "Dreambox/PostProcessing/Blur"
{
    Properties
    {
        [KeywordEnum(Box, Gaussian)] ALGORITHM ("Algorithm", Integer) = 0
        [KeywordEnum(Clamp, Mirror)] WRAP_MODE ("Wrap Mode", Integer) = 0
        _Radius ("Radius", Float) = 1
        _Factor ("Factor", Range(0, 1)) = 1 
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
            #pragma vertex Vert
            #pragma fragment FragHorizontal
            #pragma shader_feature_local_fragment ALGORITHM_BOX ALGORITHM_GAUSSIAN
            #pragma shader_feature_local_fragment WRAP_MODE_CLAMP WRAP_MODE_MIRROR
            ENDHLSL
        }

        Pass
        {
            Name "Vertical"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragVertical
            #pragma shader_feature_local_fragment ALGORITHM_BOX ALGORITHM_GAUSSIAN
            #pragma shader_feature_local_fragment WRAP_MODE_CLAMP WRAP_MODE_MIRROR
            ENDHLSL
        }
    }
}