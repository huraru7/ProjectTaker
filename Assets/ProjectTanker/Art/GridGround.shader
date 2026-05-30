Shader "Custom/GridGround"
{
    Properties
    {
        _BaseColor  ("Base Color",  Color) = (0.17, 0.24, 0.31, 1)
        _LineColor  ("Line Color",  Color) = (1, 1, 1, 0.18)
        _GridSize   ("Grid Size",   Float) = 20
        _LineWidth  ("Line Width",  Range(0.9, 0.999)) = 0.97
        _LineColor2 ("Line Color 2 (Large)", Color) = (1, 1, 1, 0.35)
        _GridSize2  ("Grid Size 2 (Large)", Float) = 5
        _LineWidth2 ("Line Width 2 (Large)", Range(0.9, 0.999)) = 0.95
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "GridGround"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _LineColor;
                float  _GridSize;
                float  _LineWidth;
                float4 _LineColor2;
                float  _GridSize2;
                float  _LineWidth2;
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

            float GridLine(float2 uv, float width)
            {
                float2 f = frac(uv);
                float2 s = step(width, f);
                return clamp(s.x + s.y, 0.0, 1.0);
            }

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

                // 小グリッド
                float lineSmall = GridLine(uv * _GridSize,  _LineWidth);
                // 大グリッド
                float lineLarge = GridLine(uv * _GridSize2, _LineWidth2);

                half4 col = _BaseColor;
                col = lerp(col, half4(_LineColor.rgb,  col.a), lineSmall * _LineColor.a);
                col = lerp(col, half4(_LineColor2.rgb, col.a), lineLarge * _LineColor2.a);

                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
