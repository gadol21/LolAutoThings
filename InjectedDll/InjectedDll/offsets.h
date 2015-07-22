#ifndef __OFFSETS_H
#define __OFFSETS_H

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x7dd910;						// updated to 5.14

const uint32_t send_chat_message = 0x944f00;				// updated to 5.14
const uint32_t send_chat_message_this = 0x2d9c548;			// updated to 5.14
const uint32_t send_chat_message_this_message_offset = 0xe8;// updated to 5.14

const uint32_t floating_text = 0x99E580;					// updated to 5.14
const uint32_t floating_text_magic = 0x111CCF0;				// updated to 5.14

const uint32_t cast_spell = 0xbf61C0;						// updated to 5.14
// this offset is from main_unit base address
const uint32_t cast_spell_spellmanager = 0x2730;

} // namespace offsets

#endif // ndef __OFFSETS_H