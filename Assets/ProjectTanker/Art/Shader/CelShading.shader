Shader "ProjectTanker/CelShading"
{
    Properties
    {
        _MainTex     ("Sprite Texture",  2D)           = "white" {}
        _BaseColor   ("Base Color",      Color)        = (0.3, 0.72, 0.91, 1)
        _ShadowColor ("Shadow Color",    Color)        = (0.18, 0.50, 0.68, 1)
        _Steps       ("Shading Steps",   Range(1, 4))  = 2
        _ShadowBias  ("Shadow Bias",     Range(0, 1))  = 0.4
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "CelShading"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _ShadowColor;
                float  _Steps;
                float  _ShadowBias;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;  // SpriteRenderer.color
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                OUT.color       = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // テクスチャの輝度をセルシェーディングのグレードとして使う。
                // 白っぽいピクセル = BaseColor、暗いピクセル = ShadowColor
                float luminance = dot(texSample.rgb, float3(0.299, 0.587, 0.114));
                float biased    = saturate((luminance - _ShadowBias) / (1.0 - _ShadowBias + 1e-5));
                float stepped   = floor(biased * _Steps + 0.5) / _Steps;

                half4 col;
                col.rgb = lerp(_ShadowColor.rgb, _BaseColor.rgb, stepped) * IN.color.rgb;
                col.a   = texSample.a * IN.color.a;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
