Shader "ProjectTanker/DotGround"
{
    Properties
    {
        _BgColor     ("Background Color", Color)            = (0.96, 0.97, 0.98, 1)
        _DotColor    ("Grid Color",        Color)            = (0.78, 0.85, 0.89, 1)
        _DotSpacing  ("Cell Size",         Float)            = 1.0
        _DotRadius   ("Line Width",        Range(0.01, 0.1)) = 0.03
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Background"
        }

        Pass
        {
            Name "DotGround"
            ZWrite On

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BgColor;
                float4 _DotColor;
                float  _DotSpacing;
                float  _DotRadius;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vpi = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vpi.positionCS;
                OUT.worldPos    = vpi.positionWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv       = IN.worldPos.xy / _DotSpacing;
                float2 cell     = frac(uv);

                // 各セルの端からの距離（縦線・横線の距離）
                float2 fromEdge = min(cell, 1.0 - cell);
                float  minDist  = min(fromEdge.x, fromEdge.y);

                float lineWidth = _DotRadius;

                float edge  = lineWidth * 0.3;
                float grid  = 1.0 - smoothstep(lineWidth - edge, lineWidth + edge, minDist);

                return lerp(_BgColor, _DotColor, grid);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
