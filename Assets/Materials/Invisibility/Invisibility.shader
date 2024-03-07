Shader "CustomPad/Invisibility" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Cor("Color Specular", Color) = (1,1,1,1)
		_Main("Texture", 2D) = "white" {}
		_MainTex("NormalMap", 2D) = "bump" {}
		_Largura("Size", Range(0,100)) = 48
		_Dx("Distortion X", Range(-1,1)) = 0
		_Dy("Distortion Y", Range(-1,1)) = 0.03
		_Velocidade("Speed", Range(0,2)) = 0.2
		_Forca("Strong", Range(0,2)) = 1
		_BrilhoSpec("Shine Specular", Range(0,10)) = 1
		_Brilho("Shine", Range(0,3)) = 2
		_TipoI("Kind Invisibility", Range(0,2)) = 1
		_Animacao("Animation", Range(-1,1)) = 1
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		GrabPass{}
		Pass
		{
			SetTexture[_GrabTexture]{ combine one - texture }
		}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SimpleSpecular
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Main;
		sampler2D _GrabTexture;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float4 screenPos;
	};

	half _Largura;
	fixed4 _Color;
	fixed4 _Cor;
	half _Dx;
	half _Dy;
	half _Velocidade;
	float _Forca;
	float _Brilho;
	half _BrilhoSpec;
	fixed _TipoI;
	float _Animacao;


	half4 LightingSimpleSpecular(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
		half4 c;
		if (dot(s.Normal, viewDir) < _Animacao) {

			half3 h = normalize(lightDir + viewDir);

			half diff = max(0, dot(s.Normal, viewDir));

			float nh = max(0, dot(s.Normal, h));
			float spec = pow(nh, _Largura);

			
			c.rgb = (s.Albedo * diff + _Cor * spec) * atten*_BrilhoSpec;
			c.a = s.Alpha;
			
		}
		else {
			c.rgb = s.Albedo;
			c.a = s.Alpha;
		}
		return c;
	}


	void surf(Input IN, inout SurfaceOutput o) {
		
		float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
		screenUV.y = 1 - screenUV.y - _Dy;
		screenUV.x = screenUV.x - _Dx;
		o.Normal = UnpackNormal(tex2D(_MainTex, screenUV - _Time.x * _Velocidade));
		if (dot(o.Normal, IN.viewDir) < _Animacao) {
			float3 c = tex2D(_GrabTexture, screenUV).rgb;
			if (_TipoI < 1) {
				float NdotV = dot(o.Normal, IN.viewDir);
				o.Emission = c * (_Brilho * dot(o.Normal, IN.viewDir) * ceil (NdotV - 0.2) + (2 * (1 - ceil(NdotV - 0.2))));
			}
			else if (_TipoI < 2) {
				o.Emission = c * (_Brilho - dot(o.Normal, IN.viewDir));
			}
			else if (_TipoI < 3) {
				float NdotV = dot(o.Normal, IN.viewDir);
				o.Emission = c * (_Brilho * dot(o.Normal, IN.viewDir) * ceil(NdotV - 0.5) + (_Forca * (1 - ceil(NdotV - 0.5))));
			}
		}else{
			o.Albedo = tex2D(_Main, IN.uv_MainTex) * _Color.rgb;
		}
		
	}
	ENDCG
	}
		FallBack "Diffuse"
}
