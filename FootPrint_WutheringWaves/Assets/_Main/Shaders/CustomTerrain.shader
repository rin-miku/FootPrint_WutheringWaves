Shader "Custom/CustomTerrain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _OverlayNormalMap ("Overlay Normal Map", 2D) = "bump" {}
        _HeightMap ("Height Map", 2D) = "white" {}
        _HeightScale ("Height Scale", Range(0,10)) = 0.5
        _BottomColor("Bottom Color", Color) = (0.8,0.8,1,1)
        _TessellationAmount("Tessellation Amount", Range(1,32)) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows tessellate:tess nolightmap

        float _TessellationAmount;
        float tess()
        {
            return _TessellationAmount;
        }

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _OverlayNormalMap;
        sampler2D _HeightMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_OverlayNormalMap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _HeightScale;
        fixed4 _BottomColor;

        void vert(inout appdata_full v)
        {
            float offset = tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0)).r * _HeightScale;
            v.vertex.y += offset;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = tex2D(_HeightMap, IN.uv_MainTex).r;
            fixed4 color = tex2D(_MainTex, IN.uv_MainTex) * lerp(_BottomColor, _Color, height);

            o.Albedo = color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = color.a;

            //o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            float3 baseNormal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            float3 overlayNormal = UnpackNormal(tex2D(_OverlayNormalMap, IN.uv_OverlayNormalMap));
            float mask = tex2D(_OverlayNormalMap, IN.uv_OverlayNormalMap).a;
            o.Normal = normalize(lerp(baseNormal, overlayNormal, mask));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
