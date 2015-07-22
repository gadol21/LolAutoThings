#ifndef __OFFSETS_H
#define __OFFSETS_H

#include <cstdint>

namespace offsets {

const uint32_t main_loop = 0x18ad60;

const uint32_t send_chat_message = 0xa412a0;

const uint32_t floating_text = 0x99E580;
const uint32_t floating_text_magic = 0x111CCF0;

const uint32_t cast_spell = 0xca40d0;

} // namespace offsets

#endif // ndef __OFFSETS_H