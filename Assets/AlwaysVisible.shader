Shader "Custom/AlwaysVisible"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Always("Always Visible Color", Color) = (0,0,0,0)
    }
     Category 
 {
     SubShader
     {
         Tags { "Queue"="Overlay+1"
         "RenderType"="Transparent"}
      
         Pass
         {
             ZWrite Off
             ZTest Greater
             Lighting Off
             Color [_Always]
         }
      
         Pass
         {
             //Blend SrcAlpha On
             Lighting On
             ZTest Less
             Color [_Color]
             SetTexture [_MainTex] {combine texture}
             _Glossiness
         }
     }
     }
    FallBack "Diffuse"
}
