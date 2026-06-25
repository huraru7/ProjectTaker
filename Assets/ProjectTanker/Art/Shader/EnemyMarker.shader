Shader "ProjectTanker/EnemyMarker"
{
    Properties
    {
        _Color     ("Color", Color)              = (0.961, 0.773, 0.094, 1)
        _Intensity ("HDR Intensity", Range(1,3)) = 1.8
        _BobSpeed  ("Bob Speed",  Range(1, 6))   = 3.0
        _BobAmount ("Bob Amount", Range(0, 0.5)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"
               "RenderPipeline"="UniversalRenderPipeline" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            half4  _Color;
            float  _Intensity;
            float  _BobSpeed;
            float  _BobAmount;

            Varyings vert(Attributes IN)
            {
                // 上下ボブアニメーション（頂点シェーダーでメッシュ全体を動かす）
                IN.positionOS.y += sin(_Time.y * _BobSpeed) * _BobAmount;

                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 p = IN.uv * 2.0 - 1.0; // -1 〜 1 にリマップ

                // ── 二重シェブロン（下向き ∨∨）──
                // 上側シェブロン
                float d1   = abs(abs(p.x) + p.y + 0.15);
                float c1   = smoothstep(0.14, 0.04, d1)
                           * step(-0.45, p.y) * step(p.y, 0.65);

                // 下側シェブロン
                float d2   = abs(abs(p.x) + p.y + 0.65);
                float c2   = smoothstep(0.14, 0.04, d2)
                           * step(-0.95, p.y) * step(p.y, 0.15);

                float mask  = saturate(c1 + c2);

                // フェードイン/アウトのパルス
                float pulse = 0.65 + 0.35 * sin(_Time.y * _BobSpeed);
                float alpha = mask * pulse;

                clip(alpha - 0.01);
                return half4(_Color.rgb * _Intensity, alpha);
            }
            ENDHLSL
        }
    }
}
