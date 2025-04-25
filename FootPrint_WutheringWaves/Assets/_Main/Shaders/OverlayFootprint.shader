Shader "Custom/OverlayFootprint"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap1 ("Main Normal Map", 2D) = "bump" {}
        _BumpMap2 ("Overlay Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BumpMap1;
        sampler2D _BumpMap2;
        sampler2D _DecalTex;
        float4 _ClickPos;
        half _Glossiness;
        half _Metallic;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap1;
            float2 uv_BumpMap2;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = col.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            float3 baseNormal = UnpackNormal(tex2D(_BumpMap1, IN.uv_BumpMap1));
            float3 overlayNormal = UnpackNormal(tex2D(_BumpMap2, IN.uv_BumpMap2));
            float mask = tex2D(_BumpMap2, IN.uv_BumpMap2).a;

            o.Normal = normalize(lerp(baseNormal, overlayNormal, mask));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
