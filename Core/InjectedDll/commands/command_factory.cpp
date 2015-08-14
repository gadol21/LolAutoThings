#include "commands/command_factory.h"
#include "commands/command_types.h"
#include "commands/send_chat_command.h"
#include "commands/floating_text_command.h"
#include "commands/cast_spell_command.h"
#include "commands/attackmove_command.h"
#include "commands/print_to_user.h"
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
	case command_type::FLOATING_TEXT:
		return CommandPtr(new FloatingTextCommand(command_without_id, command_without_id_len));
	case command_type::CAST_SPELL:
		return CommandPtr(new CastSpellCommand(command_without_id, command_without_id_len));
	case command_type::MOVEATTACK:
		return CommandPtr(new AttackMoveCommand(command_without_id, command_without_id_len));
	case command_type::PRINT_USER:
		return CommandPtr(new PrintToUserCommand(command_without_id, command_without_id_len));
	default:
		throw std::invalid_argument("unknown command");
	}
}