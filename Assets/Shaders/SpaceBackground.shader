Shader "Custom/RandomStarColorSize"
{
    Properties
    {
        _StarDensity("Star Density", Range(0.0, 1.0)) = 0.5
        _TwinklePeriodMin("Twinkle Period Min", Float) = 2.0
        _TwinklePeriodMax("Twinkle Period Max", Float) = 5.0
        _ScrollSpeed("Scroll Speed", Float) = 0.001
        _GalaxyColor1("Galaxy Color 1", Color) = (0.1, 0.2, 0.4, 1)
        _GalaxyColor2("Galaxy Color 2", Color) = (0.3, 0.6, 1.0, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _StarDensity;
            float _TwinklePeriodMin;
            float _TwinklePeriodMax;
            float _ScrollSpeed;
            float4 _GalaxyColor1;
            float4 _GalaxyColor2;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float2 hash2(float2 p)
            {
                return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    v += a * noise(p);
                    p *= 2.0;
                    a *= 0.5;
                }
                return v;
            }

            float3 starfield(float2 uv, float time)
            {
                float brightness = 0.0;
                float3 finalColor = float3(0, 0, 0);

                float tile = 40.0;
                float2 id = floor(uv * tile);
                float2 localUV = frac(uv * tile);

                float starExist = hash(id);
                if (starExist > _StarDensity) return float3(0, 0, 0);

                float2 offset = hash2(id);
                float2 starPos = offset;
                float d = distance(localUV, starPos);

                // 크기 랜덤화
                float starSize = lerp(0.01, 0.1, hash(id + 1.23));

                // 색상 랜덤화
                float3 baseColor = lerp(float3(1, 0.9, 0.8), float3(0.7, 0.8, 1), hash(id + 0.5));

                // 주기 기반 깜빡임
                float period = lerp(_TwinklePeriodMin, _TwinklePeriodMax, hash(id + 0.2));
                float phase = hash(id + 0.3) * 6.2831;
                float twinkle = sin((6.2831 * time / period) + phase) * 0.5 + 0.5;

                float b = smoothstep(starSize, 0.0, d) * twinkle;
                return baseColor * b;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y;
                float2 uv = i.uv;

                float2 scrollUV = uv + float2(0, t * _ScrollSpeed);

                // 은하수 배경
                float n1 = fbm(scrollUV * 2.0);
                float n2 = fbm(scrollUV * 4.0);
                float3 galaxy = lerp(_GalaxyColor1.rgb, _GalaxyColor2.rgb, n2) * n1;

                float3 stars = starfield(uv, t);

                return float4(galaxy + stars, 1.0);
            }
            ENDCG
        }
    }
}
