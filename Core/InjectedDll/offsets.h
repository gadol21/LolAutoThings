#pragma once

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x62b720;						// 6.22

const uint32_t list_remove = 0x715f20;						// 6.22
const uint32_t list_add = 0x1f4ae0;							// 6.22

const uint32_t league_object_name_len = 0x30;				// 6.22

const uint32_t send_chat_message = 0x1569d0;				// 6.22
const uint32_t send_chat_message_this = 0x31f85e0;			// 6.22
const uint32_t send_chat_message_this_message_offset = 0xfc;// 6.22
const uint32_t print_to_user = 0x467f60;					// 6.22

const uint32_t floating_text = 0x7a75c0;					// 6.22
const uint32_t floating_text_magic = 0x15743a8;				// 6.22
const uint32_t floating_text_visability_check = 0x5bbdcc;	// 6.22

const uint32_t cast_spell = 0x4e42d0;						// 6.22
// these offsets are from main_unit base address
const uint32_t cast_spell_this = 0x2858;					// 6.22

const uint32_t attackmove = 0x6bffb0;						// 6.22

} // namespace offsets
