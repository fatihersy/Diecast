#pragma once

#include "CommonHeaders.h"

namespace die::math {
	inline constexpr float PI = 3.1415926535897932384626433832795f;
	inline constexpr float epsilon = 1e-5f;

#if defined(_WIN64)
	using FLOAT2  = DirectX::XMFLOAT2;
	using FLOAT2A = DirectX::XMFLOAT2A;
	using FLOAT3  = DirectX::XMFLOAT3;
	using FLOAT3A = DirectX::XMFLOAT3A;
	using FLOAT4  = DirectX::XMFLOAT4;
	using FLOAT4A = DirectX::XMFLOAT4A;
	using UINT2   = DirectX::XMUINT2;
	using UINT3   = DirectX::XMUINT3;
	using UINT4   = DirectX::XMUINT4;
	using INT2	  = DirectX::XMINT2;
	using INT3	  = DirectX::XMINT3;
	using INT4	  = DirectX::XMINT4;
	using FLOAT3X3  = DirectX::XMFLOAT3X3; // DirectxMath doesn't have aligned 3x3 matrices
	using FLOAT4X4  = DirectX::XMFLOAT4X4;
	using FLOAT4X4A = DirectX::XMFLOAT4X4A;
#endif // defined(_WIN64)

}
