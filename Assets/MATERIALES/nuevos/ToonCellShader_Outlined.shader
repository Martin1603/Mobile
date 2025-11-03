Shader "Custom/ToonCellShader_Outlined"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _ShadeColor ("Shade Color", Color) = (0.1,0.1,0.1,1)
        _Cutoff ("Shading Threshold", Range(0,1)) = 0.5
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.05)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        // === OUTLINE PASS ===
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                // Extruye el vértice en dirección de la normal para crear el borde
                positionWS += normalWS * _OutlineThickness;

                OUT.positionHCS = TransformWorldToHClip(positionWS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }

        // === MAIN TOON PASS ===
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
            };

            float4 _Color;
            float4 _ShadeColor;
            float _Cutoff;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);

                float NdotL = saturate(dot(IN.normalWS, -lightDir));
                float shade = step(_Cutoff, NdotL);
                float3 finalColor = lerp(_ShadeColor.rgb, _Color.rgb, shade);

                return float4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
