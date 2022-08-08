Shader "Unlit/CrossHair"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (.25, .5, .5, 1)

		_FlipBookDimentions("FlipBookDimentions", Vector) = (2, 2, 0, 1)	// only (x,y) 
		_FlipIndex("FlipIndex", float) = 0
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
        LOD 100

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
            float4 _MainTex_ST;
			float4 _FlipBookDimentions;
			float _FlipIndex;
			float4 _Color;

			float2 GetUVWithFlipbook(float2 uv)
			{
				float x = _FlipIndex % 2;
				float y = (int)_FlipIndex / 2;

				uv.x /= _FlipBookDimentions.x;
				uv.y /= _FlipBookDimentions.y;

				uv.x += x * 1 / _FlipBookDimentions.x;
				uv.y += y * 1 / _FlipBookDimentions.y;
				return uv;
			}

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
				i.uv = GetUVWithFlipbook(i.uv);
                fixed4 col = tex2D(_MainTex, i.uv).g * _Color;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}