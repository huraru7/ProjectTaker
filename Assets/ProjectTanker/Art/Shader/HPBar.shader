Shader "ProjectTanker/HPBar"
{
    Properties
    {
        _FillAmount  ("Fill Amount",   Range(0, 1)) = 1.0
        _ColorFull   ("Color Full",    Color)       = (0.27, 0.76, 0.35, 1)
        _ColorMid    ("Color Mid",     Color)       = (0.95, 0.77, 0.06, 1)
        _ColorLow    ("Color Low",     Color)       = (0.98, 0.27, 0.27, 1)
        _BgColor     ("BG Color",      Color)       = (0.18, 0.20, 0.24, 1)
        _SegmentCount("Segment Count", Float)       = 10
        _FlashSpeed  ("Flash Speed",   Float)       = 6.0
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

        Pass
        {
            Name "HPBar"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorFull;
                float4 _ColorMid;
                float4 _ColorLow;
                float4 _BgColor;
                float  _FillAmount;
                float  _SegmentCount;
                float  _FlashSpeed;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // 塗り部分かどうか
                float filled = step(uv.x, _FillAmount);

                // 体力に応じた色（2段階 Lerp: 緑→黄→赤）
                float  t1  = clamp(_FillAmount * 2.0, 0.0, 1.0);
                float  t2  = clamp((_FillAmount - 0.5) * 2.0, 0.0, 1.0);
                half4  col = lerp(_ColorLow, _ColorMid, t1);
                col        = lerp(col, _ColorFull, t2);

                // セグメント区切り線
                float seg     = frac(uv.x * _SegmentCount);
                float segLine = step(0.93, seg);
                col.rgb      *= 1.0 - segLine * 0.35;

                // 瀕死フラッシュ（HP 20% 以下）
                float isDanger = step(_FillAmount, 0.2);
                float flash    = sin(_Time.y * _FlashSpeed) * 0.5 + 0.5;
                col.a          = lerp(1.0, flash, isDanger);

                return lerp(_BgColor, col, filled);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
