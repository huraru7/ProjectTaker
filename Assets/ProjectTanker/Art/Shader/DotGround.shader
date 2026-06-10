Shader "Custom/DotGround"
{
    Properties
    {
        _BgColor     ("Background Color", Color)            = (0.96, 0.97, 0.98, 1)
        _DotColor    ("Dot Color",        Color)            = (0.78, 0.85, 0.89, 1)
        _DotSpacing  ("Dot Spacing",      Float)            = 1.0
        _DotRadius   ("Dot Radius",       Range(0.02, 0.45)) = 0.07
        _PulseSpeed  ("Pulse Speed",      Float)            = 0.8
        _PulseAmount ("Pulse Amount",     Range(0.0, 0.3))  = 0.06
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
                float  _PulseSpeed;
                float  _PulseAmount;
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
                float2 uv     = IN.worldPos.xy / _DotSpacing;
                float2 cell   = frac(uv);
                float2 offset = cell - 0.5;
                float  dist   = length(offset);

                // 脈動（ほぼ気づかないくらいの微細な動き）
                float pulse  = sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float radius = _DotRadius + pulse * _DotRadius;

                float circle = 1.0 - smoothstep(radius - 0.015, radius + 0.015, dist);
                return lerp(_BgColor, _DotColor, circle);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
