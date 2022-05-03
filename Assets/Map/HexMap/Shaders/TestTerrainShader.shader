Shader "Custom/TestTerrainShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Terrain Texture Array", 2DArray) = "white" {}
        //---
        _Outline("Outline Width", Range(0,1)) = .1
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        //---
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Cull off
            Pass {
                CGPROGRAM
                // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members terrain)
                #pragma exclude_renderers d3d11
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.5

                UNITY_DECLARE_TEX2DARRAY(_MainTex);

                struct v2f
                {
                    float4 color : COLOR;
                    float3 terrain;
                };

                v2f vert(inout appdata_full v) {
                    v2f data;
                    data.terrain = v.texcoord2.xyz;
                    return data;
                }

                float4 GetTerrainColor(v2f IN, int index) {
                    float3 uvw = float3(IN.worldPos.xz * 0.02, IN.terrain[index]);
                    float4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvw);
                    return c * IN.color[index];//Если тут ни на что не умножать - вернётся чисто цвет текстурки
                }

                fixed4 _Color;

                fixed4 frag(v2f i) : SV_TARGET{
                    fixed4 c =
                            GetTerrainColor(i, 0) +
                            GetTerrainColor(i, 1) +
                            GetTerrainColor(i, 2);
                    c *= _Color;
                    return c;
                }
                    //IN.color - Цвет вершины (который задаётся в триангуляции)
                    //_Color - Цвет, который задаётся в материале
                    //IN.color[index] - компонента цвета (красная, синяя или зелёная), по сути, число от 0 до 1.
                    //c - будет итоговый цвет
                    //UNITY_SAMPLE_TEX2DARRAY(...) - Берёт текстуру из массива (индекс текстуры - третья координата второго аргумента), и преобразует её в цвет (ну точнее, в массив цветов для каждого пикселя, как я понимаю)
                ENDCG
            }
                //---
                Pass {
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

                        float3 normal = normalize(v.normal);
                        float3 o_offset = normal * _Outline;
                        float3 pos = v.vertex + o_offset;

                        o.pos = UnityObjectToClipPos(pos);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_TARGET{
                        return _OutlineColor;
                    }
                    ENDCG
                }
                //---
        }
        FallBack "Custom/TerrainShader"
}
