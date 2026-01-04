#pragma once
#include "CommonHeaders.h"
#include <limits>

namespace die::id {

	using id_type = u64;
	constexpr u32 generation_bits{ 32 };
	constexpr u32 index_bits{ sizeof(id_type) * 8 - generation_bits};
	constexpr id_type index_mask{ (id_type{1} << index_bits) - 1};
	constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
	constexpr id_type id_mask{ std::numeric_limits<id_type>::max() };

	using generation_type = std::conditional_t<generation_bits <= 16, std::conditional_t<generation_bits <= 8, u8, u16>, u32>;
	static_assert(sizeof(generation_type) * 8 >= generation_bits);
	static_assert( (sizeof(id_type) - sizeof(generation_type)) > 0 );

	inline bool is_valid(id_type id) 
	{
		return id != id_mask;
	}

	inline id_type index(id_type id) 
	{
		return id & index_mask;
	}

	inline id_type generation(id_type id) 
	{
		return (id >> index_bits) & generation_mask;
	}

	inline id_type new_generation(id_type id) 
	{
		generation_type gen = static_cast<generation_type>(generation(id) + 1);
		constexpr generation_type max_gen = std::numeric_limits<generation_type>::max() - static_cast< generation_type>(1);
		assert(gen < max_gen);
		return index(id) | (static_cast<id_type>(gen) << index_bits);
	}

#if _DEBUG
	namespace internal {
		struct id_base 
		{
			constexpr explicit id_base(id_type id) : _id{ id } {}
			constexpr operator id_type() const { return _id; }
		private: 
			id_type _id;
		};
	}
	#define DEFINED_TYPED_ID(name)											\
			struct name final : id::internal::id_base						\
			{																\
				constexpr explicit name(id::id_type id)						\
					: id_base{ id } {}										\
				constexpr name() : id_base { id::id_mask } {}				\
			};																\


#else
	#define DEFINED_TYPED_ID(name) using name = id::id_type;
#endif

}
