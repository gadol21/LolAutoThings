#include "commands/print_to_user.h"
#include "offsets.h"
#include "lol_helper.h"

PrintToUserCommand::PrintToUserCommand(const char* buffer, size_t buffer_len) {
	if (buffer_len < 1) {
		throw std::invalid_argument("PrintToUserCommand: buffer too short");
	}
	uint8_t message_len = buffer[0];
	if (message_len != buffer_len - sizeof(message_len)) {
		throw std::invalid_argument("PrintToUserCommand: wrong buffer length");
	}
	m_message = string(buffer + sizeof(message_len), message_len);
}

void PrintToUserCommand::operator()() {
	print_to_user_func PrintToUser = reinterpret_cast<print_to_user_func>(LolHelper::get_lol_base() + offsets::print_to_user);

	uint32_t send_chat_this = LolHelper::get_lol_base() + offsets::send_chat_message_this;
	PrintToUser(send_chat_this, m_message.c_str(), 0);
}