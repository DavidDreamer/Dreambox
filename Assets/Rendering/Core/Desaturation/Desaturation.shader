Shader "Hidden/Dreambox/Desaturation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex Vertex
            #pragma fragment Fragment

            sampler2D _MainTex;
            float Factor;
            
            struct VertexInput
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragmentInput
            {
                float2 uv : TEXCOORD0;
                float4 position : SV_POSITION;
            };

            FragmentInput Vertex(VertexInput vertexInput)
            {
                FragmentInput fragmentInput;
                fragmentInput.position = UnityObjectToClipPos(vertexInput.position);
                fragmentInput.uv = vertexInput.uv;
                return fragmentInput;
            }

            float4 Fragment(const FragmentInput fragmentInput) : SV_Target
            {
                const float3 color = tex2D(_MainTex, fragmentInput.uv);
                const float luminance = Luminance(color);
                const float4 finalColor = float4(lerp(color, luminance, Factor), 1);
                return finalColor;
            }
            ENDHLSL
        }
    }
}