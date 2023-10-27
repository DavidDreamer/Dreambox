// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Dreambox/UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        [KeywordEnum(Off, On)] Rounding("Rounding", int) = 0
        RoundingRadius ("Rounding Radius", Float) = 0.1
        
        [KeywordEnum(Off, On)] Gradient("Gradient", int) = 0
        [KeywordEnum(Horizontal, Vertical)] GradientDirection("Gradient Direction", int) = 0
        [KeywordEnum(Simple, Bidirectional)] GradientMode("Gradient Mode", int) = 0
        GradientStartColor ("Gradient Start Color", Color) = (1,1,1,1)
        GradientEndColor ("Gradient End Color", Color) = (1, 1, 1, 1)
        GradientStart ("Gradient Start", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #pragma shader_feature_local ROUNDING_OFF ROUNDING_ON
            #pragma shader_feature_local GRADIENT_OFF GRADIENT_ON
        
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;

            float RoundingRadius;

            float GradientDirection;
            float GradientMode;
            float4 GradientStartColor;
            float4 GradientEndColor;
            float GradientStart;
        
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));


                if (_UIVertexColorAlwaysGammaSpace)
                {
                    if(!IsGammaSpace())
                    {
                        v.color.rgb = UIGammaToLinear(v.color.rgb);
                    }
                }

                OUT.color = v.color * _Color;
                return OUT;
            }

            float EvaluateRounding(float2 UV, float Width, float Height)
            {
                const float radius = max(min(min(abs(RoundingRadius * 2), abs(Width)), abs(Height)), 1e-5);
                const float2 uv = abs(UV * 2 - 1) - float2(Width, Height) + radius;
                const float d = length(max(0, uv)) / radius;
                const float fwd = max(fwidth(d), 1e-5);
                return saturate((1 - d) / fwd);
            }

            float4 EvaluateGradient(const float2 uv)
            {
                float gradientFactor = GradientDirection == 0 ? uv.x : uv.y;
                gradientFactor = GradientMode == 0 ? gradientFactor : length(0.5 - gradientFactor) / 0.5;
                gradientFactor = saturate((gradientFactor - GradientStart ) / (1 - GradientStart));
                return lerp(GradientStartColor, GradientEndColor, gradientFactor);        
            }
        
            fixed4 frag(v2f IN) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                half4 color = IN.color * (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                #ifdef GRADIENT_ON
                color *= EvaluateGradient(IN.texcoord);
                #endif
                
                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                #ifdef ROUNDING_ON
                color.a *= EvaluateRounding(IN.texcoord, 1, 1);
                color.rgb *= color.a;
                #endif

                return color;
            }
        ENDCG
        }
    }
}
