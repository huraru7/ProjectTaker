Shader "ProjectTanker/AreaExplosion"
{
    Properties
    {
        _Color     ("Color", Color)              = (1, 0.45, 0.1, 1)
        _Alpha     ("Alpha", Range(0,1))         = 1.0
        _RingWidth ("Ring Width", Range(0,0.3))  = 0.07
        _Intensity ("HDR Intensity", Range(1,5)) = 2.0
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
            float  _Alpha;
            float  _RingWidth;
            float  _Intensity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * 2.0 - 1.0;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float dist = length(IN.uv);
                float ringInner = 1.0 - _RingWidth;

                // 外周リング
                float ring = smoothstep(ringInner - 0.03, ringInner, dist)
                           * smoothstep(1.02, 0.98, dist);

                // 内側の薄い塗り
                float fill = (1.0 - smoothstep(ringInner - 0.1, ringInner, dist)) * 0.12;

                float alpha = (ring + fill) * _Alpha;
                clip(alpha - 0.004);
                return half4(_Color.rgb * _Intensity, alpha);
            }
            ENDHLSL
        }
    }
}
