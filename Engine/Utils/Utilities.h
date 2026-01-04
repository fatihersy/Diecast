#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>
namespace die::utl {
	template<typename T>
	using vector = typename std::vector<T>;
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace die::utl {
	template<typename T>
	using deque = typename std::deque<T>;
}
#endif

namespace die::utl {
	// TODO: implement our own containers
}
