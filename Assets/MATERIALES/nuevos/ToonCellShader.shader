Shader "Custom/ToonCellShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _ShadeColor ("Shade Color", Color) = (0.1,0.1,0.1,1)
        _Cutoff ("Shading Threshold", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Include URP shader libraries
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

            // Properties
            float4 _Color;
            float4 _ShadeColor;
            float _Cutoff;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = normalWS;
                OUT.positionWS = positionWS;

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Get normalized light direction
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);

                // Compute dot product between normal and light direction
                float NdotL = saturate(dot(IN.normalWS, -lightDir)); // negate for Unity's left-handed light

                // Step shading (cell shading)
                float shade = step(_Cutoff, NdotL);

                // Mix base and shade colors
                float3 finalColor = lerp(_ShadeColor.rgb, _Color.rgb, shade);

                return float4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
