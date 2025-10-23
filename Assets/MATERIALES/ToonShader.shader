Shader "Custom/ToonShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Shadow Threshold", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Toon

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _ShadowColor;
        half _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = tex.rgb * _Color.rgb;
            o.Alpha = tex.a;
        }

        inline fixed4 LightingToon (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
        {
            float NdotL = dot(s.Normal, lightDir);
            fixed isLit = step(_Cutoff, NdotL); // 0 o 1
            fixed3 color = lerp(_ShadowColor.rgb, s.Albedo, isLit);
            return fixed4(color * _LightColor0.rgb * atten, s.Alpha);
        }
        ENDCG
    }

    FallBack "Diffuse"
}
