Shader "PostProcessing/Blur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Spread("Standard Deviation (Spread)", Float) = 0
		_GridSize("Grid Size", Integer) = 1
		_EdgeThreshold("Edge Threshold", Float) = 0.2
	}
		SubShader
		{
			Tags
			{
				"RenderType" = "Opaque"
				"RenderPipeline" = "UniversalPipeline"
			}

			HLSLINCLUDE

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#define E 2.71828f

			sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_TexelSize;
				uint _GridSize;
				float _Spread;
				float _EdgeThreshold;
			CBUFFER_END

			float gaussian(int x)
			{
				float sigmaSqu = _Spread * _Spread;
				return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
			}

			struct appdata
			{
				float4 positionOS : Position;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 positionCS : SV_Position;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.uv = v.uv;
				return o;
			}

			float SobelEdge(float2 uv)
			{
				float3 tc[9];
				float2 offset = _MainTex_TexelSize.xy;

				int i = 0;
				for (int y = -1; y <= 1; ++y)
				{
					for (int x = -1; x <= 1; ++x)
					{
						tc[i++] = tex2D(_MainTex, uv + float2(x, y) * offset).rgb;
					}
				}

				float3 gx = -tc[0] - 2.0 * tc[3] - tc[6] + tc[2] + 2.0 * tc[5] + tc[8];
				float3 gy = -tc[0] - 2.0 * tc[1] - tc[2] + tc[6] + 2.0 * tc[7] + tc[8];

				float edgeStrength = length(gx + gy);
				return edgeStrength > _EdgeThreshold ? 1.0 : 0.0;
			}

			ENDHLSL

			Pass
			{
				Name "Horizontal"

				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag_horizontal

				float4 frag_horizontal(v2f i) : SV_Target
				{
					float3 col = float3(0.0f, 0.0f, 0.0f);
					float gridSum = 0.0f;

					int upper = ((_GridSize - 1) / 2);
					int lower = -upper;
					for (int x = lower; x <= upper; ++x)
					{
						float gauss = gaussian(x);
						gridSum += gauss;
						float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
						col += gauss * tex2D(_MainTex, uv).xyz;
					}

					col /= gridSum;

					float edge = SobelEdge(i.uv);
					float3 orig = tex2D(_MainTex, i.uv).rgb;

					return float4(lerp(col, orig, edge), 1.0f);
				}
				ENDHLSL
			}

			Pass
			{
				Name "Vertical"

				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag_vertical

				float4 frag_vertical(v2f i) : SV_Target
				{
					float3 col = float3(0.0f, 0.0f, 0.0f);
					float gridSum = 0.0f;

					int upper = ((_GridSize - 1) / 2);
					int lower = -upper;
					for (int y = lower; y <= upper; ++y)
					{
						float gauss = gaussian(y);
						gridSum += gauss;
						float2 uv = i.uv + float2(0.0f, _MainTex_TexelSize.y * y);
						col += gauss * tex2D(_MainTex, uv).xyz;
					}

					col /= gridSum;

					float edge = SobelEdge(i.uv);
					float3 orig = tex2D(_MainTex, i.uv).rgb;

					return float4(lerp(col, orig, edge), 1.0f);
				}
				ENDHLSL
			}
		}
}
