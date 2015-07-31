#ifndef __OFFSETS_H
#define __OFFSETS_H

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x4c7ee0;						// 5.14 3

const uint32_t list_remove = 0x882d00;						// 5.14 3
const uint32_t list_add = 0x9c5590;							// 5.14 3

const uint32_t league_object_name_len = 0x30;				// 5.14 3

const uint32_t send_chat_message = 0x847390;				// 5.14 3
const uint32_t send_chat_message_this = 0x2d9c568;			// 5.14 3
const uint32_t send_chat_message_this_message_offset = 0xe8;// 5.14 3

const uint32_t floating_text = 0x2be610;					// 5.14 3
const uint32_t floating_text_magic = 0x1115a90;				// 5.14 3
const uint32_t floating_text_visability_check = 0xacccd4;	// 5.14 3

const uint32_t cast_spell = 0xbf61e0;						// 5.14 3
// these offsets are from main_unit base address
const uint32_t cast_spell_this = 0x22c8;					// 5.14 3
const uint32_t cast_spell_spellmanager = 0x27e0;			// 5.14 2
const uint32_t cast_spell_target_unit_offset = 0xf8;		// 5.14 2

const uint32_t attackmove = 0x6d4e40;						// 5.14 3

} // namespace offsets

#endif // ndef __OFFSETS_H