Shader "Hidden/Dreambox/Outline/Mask"
{
     SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "UnityCG.cginc"

            UNITY_DECLARE_TEX2D(_BaseMap);
            uniform uint _Variant;

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
                return _Variant;
            }
            ENDHLSL
        }
    }
}