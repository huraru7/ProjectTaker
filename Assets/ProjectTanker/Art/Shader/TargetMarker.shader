Shader "ProjectTanker/TargetMarker"
{
    Properties
    {
        _Color     ("Color", Color)              = (0.961, 0.773, 0.094, 1)
        _RingWidth ("Ring Width", Range(0, 0.2)) = 0.05
        _Intensity ("HDR Intensity", Range(1, 5)) = 1.8
        _PulseSpeed("Pulse Speed", Range(0.5, 5)) = 2.0
        _ScanSpeed ("Scan Speed", Range(0, 5))   = 1.2
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
            float  _RingWidth;
            float  _Intensity;
            float  _PulseSpeed;
            float  _ScanSpeed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * 2.0 - 1.0;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float dist  = length(IN.uv);
                float angle = atan2(IN.uv.y, IN.uv.x); // -PI ~ PI

                // 脈動：sin波で外周リングが大きさを変える
                float pulse     = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);
                float ringSize  = 0.78 + pulse * 0.08;
                float ringInner = ringSize - _RingWidth;

                // 外周リング
                float ring = smoothstep(ringInner - 0.02, ringInner, dist)
                           * smoothstep(ringSize  + 0.02, ringSize,  dist);

                // スキャンライン：回転する扇形の光
                float scanAngle = fmod(_Time.y * _ScanSpeed, 6.28318);
                float angleDiff = abs(fmod(angle - scanAngle + 9.42478, 6.28318) - 3.14159);
                float scan      = smoothstep(0.6, 0.0, angleDiff)
                                * smoothstep(0.0, ringSize, dist)
                                * (1.0 - smoothstep(ringSize * 0.95, ringSize, dist))
                                * 0.35;

                // 中心の小さいドット
                float center = (1.0 - smoothstep(0.05, 0.1, dist)) * 0.8;

                // 十字マーカー（横・縦の細線）
                float crossH = smoothstep(0.025, 0.012, abs(IN.uv.y))
                             * smoothstep(ringSize, ringSize * 0.15, dist)
                             * 0.4;
                float crossV = smoothstep(0.025, 0.012, abs(IN.uv.x))
                             * smoothstep(ringSize, ringSize * 0.15, dist)
                             * 0.4;

                float globalAlpha = 0.7 + 0.3 * pulse;
                float alpha = (ring + scan + center + crossH + crossV) * globalAlpha;
                clip(alpha - 0.004);
                return half4(_Color.rgb * _Intensity, alpha);
            }
            ENDHLSL
        }
    }
}
