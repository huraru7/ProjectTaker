Shader "ProjectTanker/Lightning"
{
    Properties
    {
        _Color        ("Lightning Color", Color) = (0.6, 0.85, 1.0, 1)
        _CoreColor    ("Core Color",      Color) = (1, 1, 1, 1)
        _FlickerSpeed ("Flicker Speed",   Float) = 25
        _NoiseScale   ("Noise Scale",     Float) = 6
        _Intensity    ("Bloom Intensity", Float) = 3.5
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

        Pass
        {
            Name "Lightning"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _CoreColor;
                float  _FlickerSpeed;
                float  _NoiseScale;
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

            // 簡易 1D ハッシュノイズ
            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            // 1D value noise（補間あり）
            float noise(float x)
            {
                float i = floor(x);
                float f = frac(x);
                float a = hash(i);
                float b = hash(i + 1.0);
                return lerp(a, b, smoothstep(0.0, 1.0, f));
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv   = IN.uv - 0.5;
                float  dist = length(uv) * 2.0;
                float  angle = atan2(uv.y, uv.x);

                // 不規則なフリッカー（多重sinで周期性を崩す）
                float flicker = 0.6 + 0.4 * sin(_Time.y * _FlickerSpeed) * sin(_Time.y * _FlickerSpeed * 1.7 + 1.3);

                // 角度方向にノイズをサンプリングして距離を歪ませる→電気の枝のようなギザギザ
                float n = noise(angle * _NoiseScale + _Time.y * 40.0);
                float jaggedDist = dist + (n - 0.5) * 0.3;

                // 中心の明るいコア
                float coreAlpha = smoothstep(0.25, 0.0, dist);

                // ギザギザの光の筋（フリッカーで強さが変動）
                float boltAlpha = smoothstep(0.5, 0.3, jaggedDist) * flicker;

                float alpha = saturate(coreAlpha + boltAlpha);
                clip(alpha - 0.01);

                half3 col = lerp(_Color.rgb, _CoreColor.rgb, coreAlpha);
                col *= _Intensity;

                return half4(col, alpha) * IN.color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
