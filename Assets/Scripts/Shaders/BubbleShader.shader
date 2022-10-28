Shader "Unlit/BubbleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OverlayTex ("Overlay", 2D) = "black" {}
        _Overlay_Sense("Overlay Sense", Range(.0, 1.)) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            sampler2D _OverlayTex;
            fixed _Overlay_Sense;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                fixed4 mask = tex2D(_OverlayTex, i.uv);
                //mask *= 0.7f + 0.3f * (_SinTime.w / 2 + 0.5f);
                fixed4 Result = col * lerp(1, mask, 1- _Overlay_Sense);
                return Result;
            }
            ENDCG
        }
    }
}
