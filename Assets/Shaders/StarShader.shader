Shader "Custom/StarShader" {
  Properties {
    _MainTex ("Mask image", 2D) = "white" {}
  }
  SubShader {
    Tags { "Queue"="Background" "RenderType"="Transparent"}
    Pass {
      zWrite OFF
      blend srcAlpha oneMinusSrcAlpha
      Cull Off // since the front is partially transparent,
        // we shouldn't cull the back

      CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag

      uniform sampler2D _MainTex;
      uniform float _Cutoff;

      struct vertexInput {
        float4 vertex : POSITION;
        float4 texcoord : TEXCOORD0;
        float4 color : COLOR;
      };
      struct vertexOutput {
        float4 pos : SV_POSITION;
        float4 tex : TEXCOORD0;
        float4 color : COLOR;
      };

      vertexOutput vert(vertexInput input)
      {
        vertexOutput output;

        output.tex = input.texcoord;
        output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
        output.color = input.color;
        return output;
      }

      float4 frag(vertexOutput input) : COLOR
      {
        float4 textureColor = tex2D(_MainTex, input.tex.xy);

        float4 color = input.color;
        color.a = color.a * textureColor.a;

        return color;
      }

      ENDCG
    }
  }

  // The definition of a fallback shader should be commented out
  // during development:
  // Fallback "Unlit/Transparent Cutout"
}
