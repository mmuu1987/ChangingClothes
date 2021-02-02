Shader "Unlit/HeadCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Edging("Edging",2D)="white"{}
        _Radius("radius",float)=0.3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Edging;
            float4 _MainTex_ST;
            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 col2 = tex2D(_Edging, i.uv);

                float2 center = float2(0.5f,0.5f);

                float dis = distance(i.uv,center);

                if(dis>=_Radius)
                {
                    col.a = 0;
                }

                if(col2.a>0.3)
                {
                    col=col2;
                }
                return col;
            }
            ENDCG
        }
    }
}
