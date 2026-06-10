Shader "Custom/WallTile"
{
    Properties
    {
        _Color     ("Wall Color",     Color)          = (0.36, 0.39, 0.44, 1)
        _EdgeColor ("Edge Highlight", Color)          = (0.50, 0.54, 0.60, 1)
        _EdgeWidth ("Edge Width",     Range(0.02, 0.2)) = 0.06
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
            Name "WallTile"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EdgeColor;
                float  _EdgeWidth;
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
                float  w  = _EdgeWidth;

                float edgeTop  = step(1.0 - w, uv.y);
                float edgeLeft = step(1.0 - w, uv.x);
                float isEdge   = clamp(edgeTop + edgeLeft, 0.0, 1.0);

                return lerp(_Color, _EdgeColor, isEdge);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
