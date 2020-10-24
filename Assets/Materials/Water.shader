Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Depth ("Depth", Range(0,1)) = 1.0
        _Scale ("Scale", Vector) = (1.0, 1.0, 1.0)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Jitter ("Jitter", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        half _Depth;
        half _Glossiness;
        half _Metallic;
        half _Jitter;
        fixed4 _Color;
        float3 _Scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        // Below here is translated from GLSL source.
        //
        // Cellular noise ("Worley noise") in 3D in GLSL.
        // Copyright (c) Stefan Gustavson 2011-04-19. All rights reserved.
        // This code is released under the conditions of the MIT license.
        // See LICENSE file for details.
        // https://github.com/stegu/webgl-noise

        float3 mod289(float3 x) {
            return x - floor(x * (1.0 / 289.0)) * 289.0;
        }

        float3 mod7(float3 x) {
            return x - floor(x * (1.0 / 7.0)) * 7.0;
        }

        float3 permute(float3 x) {
            return mod289((34.0 * x + 1.0) * x);
        }

        float cellular(float3 P) {
            // there's probably a better way to initialize these.
            float K = 0.142857142857; // 1/7
            float Ko = 0.428571428571; // 1/2-K/2
            float K2 = 0.020408163265306; // 1/(7*7)
            float Kz = 0.166666666667; // 1/6
            float Kzo = 0.416666666667; // 1/2-1/6*2
            float jitter = _Jitter; // smaller jitter gives more regular pattern

            float3 Pi = mod289(floor(P));
            float3 Pf = frac(P) - 0.5;

            float3 Pfx = Pf.x + float3(1.0, 0.0, -1.0);
            float3 Pfy = Pf.y + float3(1.0, 0.0, -1.0);
	        float3 Pfz = Pf.z + float3(1.0, 0.0, -1.0);

            float3 p = permute(Pi.x + float3(-1.0, 0.0, 1.0));
            float3 p1 = permute(p + Pi.y - 1.0);
            float3 p2 = permute(p + Pi.y);
            float3 p3 = permute(p + Pi.y + 1.0);

            float3 p11 = permute(p1 + Pi.z - 1.0);
            float3 p12 = permute(p1 + Pi.z);
            float3 p13 = permute(p1 + Pi.z + 1.0);

            float3 p21 = permute(p2 + Pi.z - 1.0);
            float3 p22 = permute(p2 + Pi.z);
            float3 p23 = permute(p2 + Pi.z + 1.0);

            float3 p31 = permute(p3 + Pi.z - 1.0);
            float3 p32 = permute(p3 + Pi.z);
            float3 p33 = permute(p3 + Pi.z + 1.0);

            float3 ox11 = frac(p11*K) - Ko;
            float3 oy11 = mod7(floor(p11*K))*K - Ko;
            float3 oz11 = floor(p11*K2)*Kz - Kzo; // p11 < 289 guaranteed

            float3 ox12 = frac(p12*K) - Ko;
            float3 oy12 = mod7(floor(p12*K))*K - Ko;
            float3 oz12 = floor(p12*K2)*Kz - Kzo;

            float3 ox13 = frac(p13*K) - Ko;
            float3 oy13 = mod7(floor(p13*K))*K - Ko;
            float3 oz13 = floor(p13*K2)*Kz - Kzo;

            float3 ox21 = frac(p21*K) - Ko;
            float3 oy21 = mod7(floor(p21*K))*K - Ko;
            float3 oz21 = floor(p21*K2)*Kz - Kzo;

            float3 ox22 = frac(p22*K) - Ko;
            float3 oy22 = mod7(floor(p22*K))*K - Ko;
            float3 oz22 = floor(p22*K2)*Kz - Kzo;

            float3 ox23 = frac(p23*K) - Ko;
            float3 oy23 = mod7(floor(p23*K))*K - Ko;
            float3 oz23 = floor(p23*K2)*Kz - Kzo;

            float3 ox31 = frac(p31*K) - Ko;
            float3 oy31 = mod7(floor(p31*K))*K - Ko;
            float3 oz31 = floor(p31*K2)*Kz - Kzo;

            float3 ox32 = frac(p32*K) - Ko;
            float3 oy32 = mod7(floor(p32*K))*K - Ko;
            float3 oz32 = floor(p32*K2)*Kz - Kzo;

            float3 ox33 = frac(p33*K) - Ko;
            float3 oy33 = mod7(floor(p33*K))*K - Ko;
            float3 oz33 = floor(p33*K2)*Kz - Kzo;

            float3 dx11 = Pfx + jitter*ox11;
            float3 dy11 = Pfy.x + jitter*oy11;
            float3 dz11 = Pfz.x + jitter*oz11;

            float3 dx12 = Pfx + jitter*ox12;
            float3 dy12 = Pfy.x + jitter*oy12;
            float3 dz12 = Pfz.y + jitter*oz12;

            float3 dx13 = Pfx + jitter*ox13;
            float3 dy13 = Pfy.x + jitter*oy13;
            float3 dz13 = Pfz.z + jitter*oz13;

            float3 dx21 = Pfx + jitter*ox21;
            float3 dy21 = Pfy.y + jitter*oy21;
            float3 dz21 = Pfz.x + jitter*oz21;

            float3 dx22 = Pfx + jitter*ox22;
            float3 dy22 = Pfy.y + jitter*oy22;
            float3 dz22 = Pfz.y + jitter*oz22;

            float3 dx23 = Pfx + jitter*ox23;
            float3 dy23 = Pfy.y + jitter*oy23;
            float3 dz23 = Pfz.z + jitter*oz23;

            float3 dx31 = Pfx + jitter*ox31;
            float3 dy31 = Pfy.z + jitter*oy31;
            float3 dz31 = Pfz.x + jitter*oz31;

            float3 dx32 = Pfx + jitter*ox32;
            float3 dy32 = Pfy.z + jitter*oy32;
            float3 dz32 = Pfz.y + jitter*oz32;

            float3 dx33 = Pfx + jitter*ox33;
            float3 dy33 = Pfy.z + jitter*oy33;
            float3 dz33 = Pfz.z + jitter*oz33;

            float3 d11 = dx11 * dx11 + dy11 * dy11 + dz11 * dz11;
            float3 d12 = dx12 * dx12 + dy12 * dy12 + dz12 * dz12;
            float3 d13 = dx13 * dx13 + dy13 * dy13 + dz13 * dz13;
            float3 d21 = dx21 * dx21 + dy21 * dy21 + dz21 * dz21;
            float3 d22 = dx22 * dx22 + dy22 * dy22 + dz22 * dz22;
            float3 d23 = dx23 * dx23 + dy23 * dy23 + dz23 * dz23;
            float3 d31 = dx31 * dx31 + dy31 * dy31 + dz31 * dz31;
            float3 d32 = dx32 * dx32 + dy32 * dy32 + dz32 * dz32;
            float3 d33 = dx33 * dx33 + dy33 * dy33 + dz33 * dz33;

            float3 d1a = min(d11, d12);
            d12 = max(d11, d12);
            d11 = min(d1a, d13); // Smallest now not in d12 or d13
            d13 = max(d1a, d13);
            d12 = min(d12, d13); // 2nd smallest now not in d13
            float3 d2a = min(d21, d22);
            d22 = max(d21, d22);
            d21 = min(d2a, d23); // Smallest now not in d22 or d23
            d23 = max(d2a, d23);
            d22 = min(d22, d23); // 2nd smallest now not in d23
            float3 d3a = min(d31, d32);
            d32 = max(d31, d32);
            d31 = min(d3a, d33); // Smallest now not in d32 or d33
            d33 = max(d3a, d33);
            d32 = min(d32, d33); // 2nd smallest now not in d33
            float3 da = min(d11, d21);
            d21 = max(d11, d21);
            d11 = min(da, d31); // Smallest now in d11
            d31 = max(da, d31); // 2nd smallest now not in d31
            d11.xy = (d11.x < d11.y) ? d11.xy : d11.yx;
            d11.xz = (d11.x < d11.z) ? d11.xz : d11.zx; // d11.x now smallest
            d12 = min(d12, d21); // 2nd smallest now not in d21
            d12 = min(d12, d22); // nor in d22
            d12 = min(d12, d31); // nor in d31
            d12 = min(d12, d32); // nor in d32
            d11.yz = min(d11.yz,d12.xy); // nor in d12.yz
            d11.y = min(d11.y,d12.z); // Only two more to go
            d11.y = min(d11.y,d11.z); // Done! (Phew!)
            return d11.y - d11.x; // F2 - F1 gives a more linear stone texture.
        }

        float4 mod289(float4 x) {
            return x - floor(x * (1.0 / 289.0)) * 289.0;
        }

        float4 permute(float4 x) {
            return mod289((34.0 * x + 1.0) * x);
        }

        float4 taylorInvSqrt(float4 r) {
            return 1.79284291400159 - 0.85373472095314 * r;
        }

        float3 fade(float3 t) {
            return t*t*t*(t*(t*6.0-15.0)+10.0);
        }

        float perlinNoise(float3 P) {
            float K = 0.142857142857; // 1/7
            float3 f31 = float3(1.0, 1.0, 1.0);
            float3 f30 = float3(0.0, 0.0, 0.0);
            float3 f3h = float3(0.5, 0.5, 0.5);
            float4 f41 = float4(1.0, 1.0, 1.0, 1.0);
            float4 f40 = float4(0.0, 0.0, 0.0, 0.0);
            float4 f4h = float4(0.5, 0.5, 0.5, 0.5);

            float3 Pi0 = floor(P);
            float3 Pi1 = Pi0 + f31;
            Pi0 = mod289(Pi0);
            Pi1 = mod289(Pi1);
            float3 Pf0 = frac(P);
            float3 Pf1 = Pf0 - f31;
            float4 ix = float4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
            float4 iy = float4(Pi0.y, Pi0.y, Pi1.y, Pi1.y);
            float4 iz0 = Pi0.zzzz;
            float4 iz1 = Pi1.zzzz;

            float4 ixy = permute(permute(ix) + iy);
            float4 ixy0 = permute(ixy + iz0);
            float4 ixy1 = permute(ixy + iz1);

            float4 gx0 = ixy0 * K;
            float4 gy0 = frac(floor(gx0) * K) - 0.5;
            gx0 = frac(gx0);
            float4 gz0 = f4h - abs(gx0) - abs(gy0);
            float4 sz0 = step(gz0, f40);
            gx0 -= sz0 * (step(0.0, gx0) - 0.5);
            gy0 -= sz0 * (step(0.0, gy0) - 0.5);

            float4 gx1 = ixy1 * K;
            float4 gy1 = frac(floor(gx1) * K) - 0.5;
            gx1 = frac(gx1);
            float4 gz1 = f4h - abs(gx1) - abs(gy1);
            float4 sz1 = step(gz1, f40);
            gx1 -= sz1 * (step(0.0, gx1) - 0.5);
            gy1 -= sz1 * (step(0.0, gy1) - 0.5);

            float3 g000 = float3(gx0.x,gy0.x,gz0.x);
            float3 g100 = float3(gx0.y,gy0.y,gz0.y);
            float3 g010 = float3(gx0.z,gy0.z,gz0.z);
            float3 g110 = float3(gx0.w,gy0.w,gz0.w);
            float3 g001 = float3(gx1.x,gy1.x,gz1.x);
            float3 g101 = float3(gx1.y,gy1.y,gz1.y);
            float3 g011 = float3(gx1.z,gy1.z,gz1.z);
            float3 g111 = float3(gx1.w,gy1.w,gz1.w);

            float4 norm0 = taylorInvSqrt(float4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
            g000 *= norm0.x;
            g010 *= norm0.y;
            g100 *= norm0.z;
            g110 *= norm0.w;
            float4 norm1 = taylorInvSqrt(float4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
            g001 *= norm1.x;
            g011 *= norm1.y;
            g101 *= norm1.z;
            g111 *= norm1.w;

            float n000 = dot(g000, Pf0);
            float n100 = dot(g100, float3(Pf1.x, Pf0.yz));
            float n010 = dot(g010, float3(Pf0.x, Pf1.y, Pf0.z));
            float n110 = dot(g110, float3(Pf1.xy, Pf0.z));
            float n001 = dot(g001, float3(Pf0.xy, Pf1.z));
            float n101 = dot(g101, float3(Pf1.x, Pf0.y, Pf1.z));
            float n011 = dot(g011, float3(Pf0.x, Pf1.yz));
            float n111 = dot(g111, Pf1);

            float3 fade_xyz = fade(Pf0);
            float4 n_z = lerp(float4(n000, n100, n010, n110), float4(n001, n101, n011, n111), fade_xyz.z);
            float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
            float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x); 
            return 2.2 * n_xyz;
        }

        // End credit section
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float noise1 = perlinNoise(IN.worldPos * _Scale * 4.0) / 32.0;
            float noise2 = perlinNoise(IN.worldPos * _Scale * 8.0) / 32.0;
            
            float3 pos = (IN.worldPos + float3(noise1, 0.0, noise2) + float3(_SinTime.z, 0, _CosTime.z) / 32.0) * _Scale;
            float baseNoise = 1.0 - min(cellular(pos), _Depth);
            // Albedo comes from a texture tinted by color
            float foam = smoothstep(0.95, 1.0, pow(baseNoise, 0.5));
            fixed4 c = lerp(_Color, fixed4(1.0, 1.0, 1.0, 1.0), foam);
            o.Albedo = c.rgb;
            o.Occlusion = baseNoise;
            // o.Normal = calculateNormal(pos);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
