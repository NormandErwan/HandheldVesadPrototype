Shader "Stencils/StencilMask" {
    Properties {
        _StencilMask("Stencil Mask", Int) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-100"}
        Pass {
			ColorMask 0

            Stencil {
                Ref [_StencilMask]
                Comp always
                Pass replace
            }
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                return half4(1,0,0,0);
            }
            ENDCG
        }
    }
}