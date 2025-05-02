Shader "Custom/CustomTerrain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _TrialColor("Bottom Color", Color) = (0.8,0.8,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HeightScale ("Height Scale", Range(0,10)) = 0.5
        _TessellationAmount("Tessellation Amount", Range(1,32)) = 8
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _OverlayNormalMap ("Overlay Normal Map", 2D) = "bump" {}
        _HeightMap ("Height Map", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma target 4.6
        #pragma surface surf Standard vertex:vert tessellate:tess addshadow

        float _TessellationAmount;
        float tess()
        {
            return _TessellationAmount;
        }

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_OverlayNormalMap;
        };

        fixed4 _Color;
        fixed4 _TrialColor;
        half _Glossiness;
        half _Metallic;
        float _HeightScale;
        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _OverlayNormalMap;
        sampler2D _HeightMap;

        void vert(inout appdata_full v)
        {
            float offset = tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0)).r * _HeightScale;
            v.vertex.y += offset;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = tex2D(_HeightMap, IN.uv_MainTex).r;
            fixed4 color = tex2D(_MainTex, IN.uv_MainTex) * lerp(_TrialColor, _Color, height);

            o.Albedo = color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = color.a;

            float3 baseNormal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            float3 overlayNormal = UnpackNormal(tex2D(_OverlayNormalMap, IN.uv_OverlayNormalMap));
            float mask = tex2D(_OverlayNormalMap, IN.uv_OverlayNormalMap).a;
            o.Normal = normalize(lerp(baseNormal, overlayNormal, mask));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
