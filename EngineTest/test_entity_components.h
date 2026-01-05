
#pragma warning(disable: 5260)
#include "test.h"
#include "Entity.h"
#include "Transform.h"

#include <iostream>
#include <ctime>
#include <random>
#include <limits>

using namespace die;

class engine_test : public test {
public:
	bool initialize() override 
	{
		return true; 
	}
	void run() override 
	{ 
		do {
			for (u32 itr{0}; itr < 10000; ++itr)
			{
				create_random();
				remove_random();
				_num_entities = (u32)_entities.size();
			}
			print_results();
		} while (getchar() != 'q');
	}
	void shutdown() override 
	{ }
private:

	utl::vector<game_entity::entity> _entities;

	u32 _added{};
	u32 _removed{};
	u32 _num_entities{};


	void create_random() {
		static std::random_device rd;
		static std::mt19937_64 gen(rd());
		static std::uniform_real_distribution<float> dist(0, 20);
		u32 count = static_cast<u32>(dist(gen));

		transform::init_info transform_info{};
		game_entity::entity_info entity_info { &transform_info };

		if (_entities.empty()) count = 1000;
		while(count > 0){
			++_added;

			game_entity::entity entity{ game_entity::create_game_entity(entity_info) };
			assert(entity.is_valid() && id::is_valid(entity.get_id()));
			_entities.push_back(entity);
			assert(game_entity::is_alive(entity));

			--count;
		}
	}
	void remove_random() {
		if (_entities.size() < 1000) return;

		static std::random_device rd;
		static std::mt19937_64 gen(rd());
		static std::uniform_real_distribution<float> remove_count_dist(0, 20);
		u32 count = static_cast<u32>(remove_count_dist(gen));

		while (count > 0) {
			std::uniform_real_distribution<float> entity_index_removal_dist(0.f, static_cast<f32>(_entities.size()));
			
			const u32 index{ static_cast<u32>(entity_index_removal_dist(gen)) };
			const game_entity::entity entity{ _entities[index] };
			assert(entity.is_valid() && id::is_valid(entity.get_id()));

			if (entity.is_valid())
			{
				game_entity::remove_game_entity(entity);
				_entities.erase(_entities.begin() + index);
				assert(!game_entity::is_alive(entity));
				++_removed;
			}
			
			--count;
		}
	}

	void print_results()
	{
		std::cout << "Entities created: " << _added << "\n";
		std::cout << "Entities deleted: " << _removed << "\n";
	}

};





