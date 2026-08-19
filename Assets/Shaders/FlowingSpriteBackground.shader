Shader "Custom/FlowingSpriteBackground"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _ScrollSpeed("Vertical Scroll Speed", Float) = 1.0
        _Color("Tint", Color) = (1, 1, 1, 1)
        [MaterialToggle] PixelSnap("Pixel Snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1, 1, 1, 1)
        [HideInInspector] _Flip("Flip", Vector) = (1, 1, 1, 1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="False"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile_instancing
            #include "UnitySprites.cginc"

            float _ScrollSpeed;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 scrollUV = i.texcoord;
                scrollUV.y = frac(scrollUV.y + _Time.y * _ScrollSpeed);

                return SampleSpriteTexture(scrollUV) * i.color;
            }
            ENDCG
        }
    }
}
