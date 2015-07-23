#ifndef __OFFSETS_H
#define __OFFSETS_H

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x7dd910;						// 5.14

const uint32_t send_chat_message = 0x944f00;				// 5.14
const uint32_t send_chat_message_this = 0x2d9c548;			// 5.14
const uint32_t send_chat_message_this_message_offset = 0xe8;// 5.14

const uint32_t floating_text = 0x99E580;					// 5.14
const uint32_t floating_text_magic = 0x111CCF0;				// 5.14

const uint32_t cast_spell = 0xbf61C0;						// 5.14
// these offsets are from main_unit base address
const uint32_t cast_spell_this = 0x22c8;					// 5.14
const uint32_t cast_spell_spellmanager = 0x27e0;			// 5.14
const uint32_t cast_spell_target_unit_offset = 0xf8;		// 5.14

const uint32_t attackmove = 0x5f65d0;						// 5.14

} // namespace offsets

#endif // ndef __OFFSETS_H