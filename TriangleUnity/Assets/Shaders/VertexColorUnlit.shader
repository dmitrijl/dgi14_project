Shader "Custom/Vertex Color Unlit" {
    Properties {
    }
    SubShader {
        Pass {
      Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct vertexInput {
                half4 vertex : POSITION;
                half4 color : COLOR;
            };

            struct vertexOutput{
                half4 pos : SV_POSITION;
                half4 color : COLOR;
            };
            
            vertexOutput vert(vertexInput input) {
                vertexOutput output;

        output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
        output.color = input.color;

                return output;
            }
            
            float4 frag(vertexOutput input) : COLOR {
                return input.color;
            }
            
            ENDCG
        }
    }
}