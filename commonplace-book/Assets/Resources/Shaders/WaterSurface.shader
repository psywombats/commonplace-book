Shader "Commonplace/WaterSurface"
{
	Properties
	{
        _Var("Var", Range(-1,1)) = 0.5
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
                float4 screenPos;
                float eyeDepth;
			};

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                COMPUTE_EYEDEPTH(o.eyeDepth);
            }
            
            uniform float _Var;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
			    half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(
                        _CameraDepthTexture,
                        float4(IN.screenPos.x, IN.screenPos.y, IN.screenPos.z, IN.screenPos.w)));
			    float deep = saturate(depth - IN.screenPos.w);
                
                float alpha = saturate(smoothstep(0, 2, deep) + .8);
                
                float r = 0;
                if (deep < .05) r = 1;
                
                float4 c = float4(r, r, r, alpha);
                o.Albedo = c.rgb * c.a;
                o.Alpha = c.a;
            }
		ENDCG
	}
}
