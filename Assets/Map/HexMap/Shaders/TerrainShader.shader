Shader "Custom/TerrainShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("Terrain Texture Array", 2DArray) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
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
            //float2 uv_MainTex;
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
            return IN.color;//c * IN.color[index];// * IN.color;//c * IN.color[index]; //���� ��� �� �� ��� �� �������� - �������� ����� ���� ���������
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //IN.color - ���� ������� (������� ������� � ������������)
            //_Color - ����, ������� ������� � ���������
            //IN.color[index] - ��� ��� �����, ���-�� ������, ���-�� �����, �� ����� - �����. ����� �����-��.
            //c - �������� ����
            //UNITY_SAMPLE_TEX2DARRAY(...) - ���� �������� �� ������� (������ �������� - ������ ���������� ������� ���������), � ����������� � � ���� (�� ������, � ������ ������ ��� ������� �������, ��� � �������)
            /*fixed4 c =
                (GetTerrainColor(IN, 0) +
                GetTerrainColor(IN, 1) +
                GetTerrainColor(IN, 2)) / 3.0;*/
            fixed4 c =
                    //GetTerrainColor(IN, 0) +
                    //GetTerrainColor(IN, 1) +
                    GetTerrainColor(IN, 2);
            o.Albedo = c.rgb;// *_Color;
            /*o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;*/
            //float2 uv = IN.worldPos.xz * 0.02;
            //fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(uv, 0)) * _Color;// * IN.color; //IN.color - ���� ������� (������� ������� � ������������)
            //o.Albedo = c.rgb;// * _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
