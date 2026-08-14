Shader "Custom/Sprite Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width (Pixels)", Range(0, 16)) = 1
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
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile_instancing

            #include "UnitySprites.cginc"

            fixed4 _OutlineColor;
            float _OutlineWidth;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 spriteTexture = SampleSpriteTexture(i.texcoord);
                fixed4 sprite = spriteTexture * i.color;
                float2 offset = _MainTex_TexelSize.xy * _OutlineWidth;

                fixed outlineAlpha = 0;
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2(-offset.x, 0)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2( offset.x, 0)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2(0, -offset.y)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2(0,  offset.y)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2(-offset.x, -offset.y)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2(-offset.x,  offset.y)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2( offset.x, -offset.y)).a);
                outlineAlpha = max(outlineAlpha, SampleSpriteTexture(i.texcoord + float2( offset.x,  offset.y)).a);

                fixed4 outline = _OutlineColor;
                outline.a *= outlineAlpha * (1 - spriteTexture.a) * i.color.a;
                outline.rgb *= outline.a;

                sprite.rgb *= sprite.a;
                return sprite + outline;
            }
            ENDCG
        }
    }
}
