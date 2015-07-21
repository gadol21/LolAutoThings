#include "floating_text_command.h"
#include "offsets.h"
#include "lol_helper.h"

FloatingTextCommand::FloatingTextCommand(const char* buffer, size_t buffer_len) {
	const floating_text_message* message = reinterpret_cast<const floating_text_message*>(buffer);

	size_t size_of_everything_but_string = sizeof(message->unit) + sizeof(message->type) + sizeof(message->message_len);
	if (buffer_len < size_of_everything_but_string || message->message_len != buffer_len - size_of_everything_but_string) {
		throw std::invalid_argument("FloatingText: wrong buffer length!");
	}

	m_unit = message->unit;
	m_type = message->type;
	m_message = string(message->message, message->message_len);
}

void FloatingTextCommand::operator()() {
	floating_text_func FloatingText = reinterpret_cast<floating_text_func>(LolHelper::get_lol_base() + offsets::floating_text);
	DWORD magic = LolHelper::get_lol_base() + offsets::floating_text_magic;

	FloatingText(magic, m_unit, m_type, m_message.c_str());
}