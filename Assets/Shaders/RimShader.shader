Shader "Custom/GlassField" {
	Properties {
    _Color ("Main Color", Color) = (1.0,1.0,1.0,1.0)
		_RimPower ("Rim Power", Range(0.01, 100)) = 2.0
	}
	SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent"}
		pass{
      		zWrite OFF
			blend srcAlpha oneMinusSrcAlpha
			cull back
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			uniform float4 _Color;
			uniform float _RimPower;
 
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
 
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};
 
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
 
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.normalDir = mul(_Object2World, float4(v.normal, 0.0)).xyz;
				o.posWorld = mul(_Object2World, v.vertex);
				return o;
			}
 
			half4 frag(vertexOutput i) : COLOR
			{
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				half3 normalDirection = i.normalDir;
 
				half rim =(pow(1 - saturate(dot(viewDirection.xyz, normalDirection.xyz)), _RimPower));
 
				return half4(_Color.rgb, rim * _Color.a);
			}
			ENDCG
		}
 
	}
	FallBack "Diffuse"
}