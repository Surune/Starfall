Shader "Custom/Sprite Outline"
{
    Properties
    {
        [Header(Tint)]
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        [KeywordEnum(Multiply, Additive, Screen, Replace)] _TintBlend("Tint Blend Mode", Float) = 0
        _TintSourceColor("Tint Source Color", Color) = (1, 1, 1, 1)
        _TintSourceRange("Tint Source Range", Range(0.01, 1.73)) = 0.35

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width (Pixels)", Range(0, 16)) = 1
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
            #pragma vertex SpriteOutlineVert
            #pragma fragment frag
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile_instancing
            #pragma shader_feature_local_fragment _TINTBLEND_MULTIPLY _TINTBLEND_ADDITIVE _TINTBLEND_SCREEN _TINTBLEND_REPLACE

            #include "UnitySprites.cginc"

            fixed4 _OutlineColor;
            float _OutlineWidth;
            fixed4 _TintSourceColor;
            float _TintSourceRange;
            float4 _MainTex_TexelSize;

            v2f SpriteOutlineVert(appdata_t input)
            {
                v2f output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.vertex = UnityFlipSprite(input.vertex, _Flip);
                output.vertex = UnityObjectToClipPos(output.vertex);
                output.texcoord = input.texcoord;
                output.color = input.color * _RendererColor;

                return output;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 spriteTexture = SampleSpriteTexture(i.texcoord);
                fixed3 spriteColor = spriteTexture.rgb;
                fixed colorDistance = distance(spriteColor, _TintSourceColor.rgb);
                fixed colorMask = 1 - saturate(colorDistance / _TintSourceRange);
                fixed3 tintedColor;

                #if _TINTBLEND_ADDITIVE
                tintedColor = spriteColor + _Color.rgb;
                #elif _TINTBLEND_SCREEN
                tintedColor = 1 - (1 - spriteColor) * (1 - _Color.rgb);
                #elif _TINTBLEND_REPLACE
                tintedColor = _Color.rgb;
                #else
                tintedColor = spriteColor * _Color.rgb;
                #endif

                spriteColor = lerp(spriteColor, tintedColor, colorMask);
                fixed4 sprite = fixed4(spriteColor * i.color.rgb, spriteTexture.a * _Color.a * i.color.a);
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
                outline.a *= outlineAlpha * (1 - spriteTexture.a) * _Color.a * i.color.a;
                outline.rgb *= outline.a;

                sprite.rgb *= sprite.a;
                return sprite + outline;
            }
            ENDCG
        }
    }
}
