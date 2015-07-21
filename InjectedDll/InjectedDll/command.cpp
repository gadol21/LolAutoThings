#include "command.h"
#include "command_types.h"
#include <string>
#include <cstdint>

CommandPtr Command::Create(const char* command, size_t command_len) {
	if (command_len < 4) {
		throw std::invalid_argument("buffer is too short");
	}
	switch (*reinterpret_cast<const uint32_t*>(command)) {
	case command_type::SEND_CHAT:

		break;
	case command_type::FLOATING_TEXT:

		break;
	case command_type::CAST_SPELL:

		break;
	case command_type::MOVEATTACK:

		break;
	default:
		throw std::invalid_argument("unknown command");
	}
}