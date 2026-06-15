Shader "ProjectTanker/ExpOrb"
{
    Properties
    {
        _Color       ("Orb Color",   Color)        = (0.08, 0.98, 0.22, 1)
        _PulseSpeed  ("Pulse Speed", Float)        = 2.5
        _PulseAmount ("Pulse Scale", Range(0,0.2)) = 0.08
        _Intensity   ("Bloom Intensity", Float)    = 3.0
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

        // 通常アルファ合成（白背景でも不透明に見える）
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ExpOrb"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float  _PulseSpeed;
                float  _PulseAmount;
                float  _Intensity;
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
                float2 uv  = IN.uv - 0.5;
                float dist = length(uv) * 2.0;

                // サイズパルス（全体がわずかに拡縮）
                float pulse = 1.0 + _PulseAmount * sin(_Time.y * _PulseSpeed);
                float d = dist / pulse;

                // ボディ（d < 0.9 付近、ソフトエッジ）
                float bodyAlpha = smoothstep(1.0, 0.82, d);

                // 外周グロー（d = 0.85〜1.4 付近に広がる光輪）
                float glowAlpha = pow(saturate((1.4 - d) / 0.6), 2.5) * 0.55;

                float alpha = saturate(bodyAlpha + glowAlpha);
                clip(alpha - 0.005);

                // 球体シェーディング: 中心 1.3 倍 → エッジ 0.5 倍
                float shade = lerp(0.5, 1.3, pow(saturate(1.0 - d), 0.65));
                half3 col = _Color.rgb * shade;

                // 白いスペキュラハイライト（左上にオフセット）
                float spec = smoothstep(0.3, 0.03, length(uv + float2(0.1, 0.1)) * 2.0);
                col += spec * 0.9;

                // HDR 出力: _Intensity > 1 でブルームに寄与する
                col *= _Intensity;

                // SpriteRenderer の Color を反映
                return half4(col, alpha) * IN.color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
