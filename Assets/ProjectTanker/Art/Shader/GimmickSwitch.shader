Shader "ProjectTanker/GimmickSwitch"
{
    Properties
    {
        _OffColor   ("Off Color",   Color)       = (0.55, 0.05, 0.05, 1)
        _OnColor    ("On Color",    Color)       = (0.10, 0.90, 0.25, 1)
        _GlowAmount ("Glow Amount", Range(0, 1)) = 0.6
        _IsOn       ("Is On",       Float)       = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "RenderType"      = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline"  = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "GimmickSwitch"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OffColor;
                float4 _OnColor;
                float  _GlowAmount;
                float  _IsOn;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color       : COLOR;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color       = IN.color;
                OUT.uv          = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv   = IN.uv - 0.5;
                float  dist = length(uv) * 2.0;

                // LED 本体（円形）
                float body = smoothstep(1.0, 0.75, dist);

                // ON 時のみ外周グロー
                float glow  = pow(saturate((1.5 - dist) / 0.6), 3.0) * _GlowAmount * _IsOn;
                float alpha = saturate(body + glow);
                clip(alpha - 0.005);

                // OFF: 暗赤色, ON: 明るい緑
                half3 col     = lerp(_OffColor.rgb, _OnColor.rgb, _IsOn);
                float hotspot = smoothstep(0.6, 0.0, dist) * _IsOn * 0.5;
                col = (col + hotspot) * lerp(0.45, 1.0, _IsOn);

                return half4(col, alpha) * IN.color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
