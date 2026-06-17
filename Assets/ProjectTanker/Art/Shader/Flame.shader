Shader "ProjectTanker/Flame"
{
    Properties
    {
        _BaseColor   ("Base Color (bottom)", Color) = (1, 0.85, 0.2, 1)
        _TipColor    ("Tip Color (top)",     Color) = (1, 0.25, 0.05, 1)
        _ScrollSpeed ("Scroll Speed",        Float) = 1.5
        _NoiseScale  ("Noise Scale",         Float) = 4
        _Intensity   ("Bloom Intensity",     Float) = 2.5
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
            Name "Flame"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _TipColor;
                float  _ScrollSpeed;
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

            // 簡易 2D ハッシュノイズ
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            // 2D value noise（補間あり）
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                float2 u = smoothstep(0.0, 1.0, f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // ノイズを上方向へスクロールしてメラメラ感を出す
                float2 scrolledUV = uv + float2(0.0, -_Time.y * _ScrollSpeed);
                float  n = noise(scrolledUV * _NoiseScale);

                // 横方向：中心が太く、ノイズで輪郭が揺れる
                float widthMask = smoothstep(0.5, 0.15, abs(uv.x - 0.5) + (n - 0.5) * 0.3);

                // 縦方向：上に行くほど消える、ノイズでギザギザにフェード
                float heightMask = smoothstep(1.0, 0.55, uv.y + (n - 0.5) * 0.25);

                float alpha = saturate(widthMask * heightMask);
                clip(alpha - 0.01);

                half3 col = lerp(_BaseColor.rgb, _TipColor.rgb, saturate(uv.y * 1.3));
                col *= _Intensity;

                return half4(col, alpha) * IN.color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
