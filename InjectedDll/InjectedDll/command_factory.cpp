#include "command_factory.h"
#include "command_types.h"
#include "send_chat_command.h"
#include "floating_text_command.h"
#include <string>
#include <cstdint>

CommandPtr CommandFactory::Create(const char* command, size_t command_len) {
	if (command_len < 4) {
		throw std::invalid_argument("buffer is too short");
	}
	const char* command_without_id = command + 4;
	size_t command_without_id_len = command_len - 4;

	switch (*reinterpret_cast<const uint32_t*>(command)) {
	case command_type::SEND_CHAT:
		return CommandPtr(new SendChatCommand(command_without_id, command_without_id_len));
		break;
	case command_type::FLOATING_TEXT:
		return CommandPtr(new FloatingTextCommand(command_without_id, command_without_id_len));
		break;
	case command_type::CAST_SPELL:
		throw std::invalid_argument("Not Implemented");
		break;
	case command_type::MOVEATTACK:
		throw std::invalid_argument("Not Implemented");
		break;
	default:
		throw std::invalid_argument("unknown command");
	}
}