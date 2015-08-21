#ifndef __OFFSETS_H
#define __OFFSETS_H

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x850230;						// 5.16

const uint32_t list_remove = 0xbeca50;						// 5.16
const uint32_t list_add = 0x2dd220;							// 5.16

const uint32_t league_object_name_len = 0x30;				// 5.16

const uint32_t send_chat_message = 0x2497d0;				// 5.16
const uint32_t send_chat_message_this = 0x2e0f9c8;			// 5.16
const uint32_t send_chat_message_this_message_offset = 0xe8;// 5.16
const uint32_t print_to_user = 0x1b3c20;					// 5.16

const uint32_t floating_text = 0xa0aa20;					// 5.16
const uint32_t floating_text_magic = 0x118b5f0;				// 5.16
const uint32_t floating_text_visability_check = 0x45192e;	// 5.16

const uint32_t cast_spell = 0xb57340;						// 5.16
// these offsets are from main_unit base address
const uint32_t cast_spell_this = 0x2320;					// 5.16
const uint32_t cast_spell_target_unit_offset = 0xf8;		// 5.16

const uint32_t attackmove = 0xa90e60;						// 5.16

} // namespace offsets

#endif // ndef __OFFSETS_H