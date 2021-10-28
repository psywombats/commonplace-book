Shader "Commonplace/WaterSurface"
{
	Properties
	{
        _Var("Var", Range(0, 15)) = 0.5
        _NoiseTex("Noise texture", 2D) = "white" {}
        _ReflectionTex("Reflection texture", 2D) = "black" {}
        _Smoothness("Smoothness", Range(0,1)) = 1
		_Metallic("Metallic", Range(0,1)) = 0
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

		GrabPass
		{
			"_GrabTex"
		}
		CGPROGRAM
			#pragma surface surf Standard vertex:vert fullforwardshadows keepalpha
            #pragma target 3.5     
			#pragma shader_feature VERTEX
            
			
            uniform sampler2D _CameraDepthTexture;
            
			struct Input
			{
				float4 color    : COLOR;
                float3 worldNormal; INTERNAL_DATA
                float3 worldPos;
                float4 screenPos;
                float eyeDepth;
			};

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                COMPUTE_EYEDEPTH(o.eyeDepth);
            }
            
            uniform float _Var;
            uniform float _Smoothness;
            uniform float _Metallic;
            sampler2D _NoiseTex;
            sampler2D _ReflectionTex;
            sampler2D _GrabTex;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float3 worldNormal = WorldNormalVector(IN, o.Normal);
            
                float vertFlow = _Time[1] * .2;
                float noise = tex2D(_NoiseTex, float2(IN.worldPos.x + vertFlow, IN.worldPos.z + vertFlow) * .5);
                
			    half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(
                        _CameraDepthTexture,
                        float4(IN.screenPos.x, IN.screenPos.y, IN.screenPos.z, IN.screenPos.w)));
			    float deep = saturate(depth - IN.screenPos.w);
                
                float alpha = saturate(smoothstep(0, 2, deep) + .8);
                
                float r = 0;
                if (deep < lerp(0, .1, noise)) {
                    r = 1;
                }
                float g = saturate(smoothstep(.4, 3, deep));
                
                half4 bgcolor = tex2Dproj(_GrabTex, float4(IN.screenPos.x ,IN.screenPos.y + noise * .1, IN.screenPos.z, IN.screenPos.w));
                bgcolor *= saturate(smoothstep(1, .5, deep));
                bgcolor *= (1 - alpha * .5);
                
                //half4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(IN.screenPos + noise * .1));
			    //rtReflections *= dot(o.Normal, worldNormal.y);
                
                float4 c = float4(r, r + g, r, alpha);
                c.r += bgcolor.r;
                c.g += bgcolor.g;
                c.b += bgcolor.b;
                //c.r = rtReflections.r;
                //c.g = rtReflections.g;
                //c.b = rtReflections.b;
                
                o.Albedo = c.rgb * c.a;
                o.Alpha = c.a;
                o.Smoothness = _Smoothness;
			    o.Metallic = _Metallic;
            }
		ENDCG
	}
}
