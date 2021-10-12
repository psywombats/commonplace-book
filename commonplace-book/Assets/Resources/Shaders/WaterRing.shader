Shader "Commonplace/WaterRing"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Range(0,1)) = 0.25
		_Metallic("Metallic", Range(0,1)) = 0.25
        _NoiseTex("Noise texture", 2D) = "white" {}
        _Rate("Dispersion rate", Range(0,5)) = 1
        _Width("Bar width", Range(0, 0.2)) = 0.05
        _WidthFudge("Bar width fudge", Range(0, .05)) = 0.01
        _Variance("Variance", Range(0, .5)) = 0.1
        _Elapsed("Elapsed", Range(0, 1)) = 0
    }
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"PreviewType"="Plane"
		}

		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
			#pragma surface surf Standard vertex:vert fullforwardshadows keepalpha
            #pragma target 3.5     
			#pragma shader_feature VERTEX

			struct Input
			{
                float2 uv_MainTex;
                float2 texcoord : TEXCOORD0;
				float4 color    : COLOR;
			};
            
            float4 _Color;
            sampler2D _NoiseTex;
            float _Rate;
            float _Width;
            float _WidthFudge;
            float _Smoothness, _Metallic;
            float _Variance;
            float _Elapsed;

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                o.color = v.color * _Color;
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float2 xy = IN.uv_MainTex - float2(.5, .5);
                fixed4 col = _Color;
                
                float t = _Elapsed;
                float vertFlow = -t;
                float noise = tex2D(_NoiseTex, float2(xy.x + vertFlow, xy.y + vertFlow) * .5);

                float dist = sqrt(xy.x * xy.x + xy.y * xy.y);
                float start = t * _Rate + noise.r * _Variance;
                float end = t * _Rate + _Width + noise.r * _Variance;
                
                float4 c = float4(0, 0, 0, 0);
                if (dist > start && dist < end) {
                    c = _Color;
                }
                if (dist > start - _WidthFudge && dist < end + _WidthFudge) {
                    c = _Color * .5;
                }
                float off = saturate(smoothstep(.5, .4, dist));
                c *= off;

                o.Albedo = c.rgb;
                o.Alpha = c.a;
                o.Smoothness = _Smoothness;
			    o.Metallic = _Metallic;
            }
        ENDCG
    }
}