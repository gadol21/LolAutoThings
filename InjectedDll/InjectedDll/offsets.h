#ifndef __OFFSETS_H
#define __OFFSETS_H

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x3e67b0;						// 5.15

const uint32_t list_remove = 0x8b5110;						// 5.15
const uint32_t list_add = 0x9f2dd0;							// 5.15

const uint32_t league_object_name_len = 0x30;				// 5.15

const uint32_t send_chat_message = 0xa8ec00;				// 5.15
const uint32_t send_chat_message_this = 0x2d54738;			// 5.15
const uint32_t send_chat_message_this_message_offset = 0xe8;// 5.15

const uint32_t floating_text = 0x7fe970;					// 5.15
const uint32_t floating_text_magic = 0x10d5840;				// 5.15
const uint32_t floating_text_visability_check = 0x9c3624;	// 5.15

const uint32_t cast_spell = 0x67cf70;						// 5.15
// these offsets are from main_unit base address
const uint32_t cast_spell_this = 0x22c8;					// 5.15
const uint32_t cast_spell_spellmanager = 0x27e0;			// 5.14 2
const uint32_t cast_spell_target_unit_offset = 0xf8;		// 5.14 2

const uint32_t attackmove = 0xa18d50;						// 5.15

} // namespace offsets

#endif // ndef __OFFSETS_H