Shader "Custom/LineShader" {
	Properties {
		_Color ("Main Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		
		pass {
      zWrite OFF
      blend srcAlpha oneMinusSrcAlpha
      cull back
      
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
 
      uniform float4 _Color;
 
      struct vertexInput
      {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
      };
 
      struct vertexOutput
      {
        float4 pos : SV_POSITION;
        float4 posWorld : TEXCOORD0;
      };
 
      vertexOutput vert(vertexInput v)
      {
        vertexOutput o;
 
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        return o;
      }
 
      half4 frag(vertexOutput i) : COLOR
      {
        return half4(_Color.rgba);
      }
      ENDCG
    }
	} 
	FallBack "Diffuse"
}
