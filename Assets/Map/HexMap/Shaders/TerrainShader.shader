Shader "Custom/TerrainShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("Terrain Texture Array", 2DArray) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
            //---
       //_Outline("Outline Width", Range(0,1)) = .1
       //_OutlineColor("Outline Color", Color) = (1,1,1,1)
            //---
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows vertex:vert

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.5

            UNITY_DECLARE_TEX2DARRAY(_MainTex);

            struct Input
            {
                float4 color : COLOR;
                float3 worldPos;
                float3 terrain;
            };

            void vert(inout appdata_full v, out Input data) {
                UNITY_INITIALIZE_OUTPUT(Input, data);
                data.terrain = v.texcoord2.xyz;
            }

            float4 GetTerrainColor(Input IN, int index) {
                float3 uvw = float3(IN.worldPos.xz * 0.02, IN.terrain[index]);
                float4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvw);
                return c * IN.color[index];//Если тут ни на что не умножать - вернётся чисто цвет текстурки
            }

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                //IN.color - Цвет вершины (который задаётся в триангуляции)
                //_Color - Цвет, который задаётся в материале
                //IN.color[index] - компонента цвета (красная, синяя или зелёная), по сути, число от 0 до 1.
                //c - будет итоговый цвет
                //UNITY_SAMPLE_TEX2DARRAY(...) - Берёт текстуру из массива (индекс текстуры - третья координата второго аргумента), и преобразует её в цвет (ну точнее, в массив цветов для каждого пикселя, как я понимаю)

                fixed4 c =
                        GetTerrainColor(IN, 0) +
                        GetTerrainColor(IN, 1) +
                        GetTerrainColor(IN, 2);
                o.Albedo = c.rgb * _Color;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG

                /*
                Pass{
                    Cull front

                    CGPROGRAM
                    #include "UnityCG.cginc"

                    #pragma vertex vert
                    #pragma fragment frag

                    struct appdata {
                        float4 vertex : POSITION;
                        float3 normal : NORMAL;
                    };

                    struct v2f {
                        float4 pos : SV_POSITION;
                    };

                    uniform float _Outline;
                    uniform fixed4 _OutlineColor;

                    v2f vert(appdata v) {
                        v2f o;

                        //float3 normal = normalize(v.normal);
                        //float3 o_offset = normal * _Outline;
                        //float3 pos = v.vertex + o_offset;
                        //o.pos = UnityObjectToClipPos(pos);

                        v.vertex *= (1 + _Outline);
                        //v.vertex.xy *= (1 + _Outline);
                        //v.vertex[1] *= (1 + _Outline);
                        o.pos = UnityObjectToClipPos(v.vertex);
                        //o.pos = v.vertex;
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_TARGET{
                        return _OutlineColor;
                    }
                    ENDCG
            }*/
                //---
    }
    FallBack "Diffuse"
}
