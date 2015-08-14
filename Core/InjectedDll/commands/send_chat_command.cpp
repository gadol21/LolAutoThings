#define _CRT_SECURE_NO_WARNINGS

#include "send_chat_command.h"
#include "lol_helper.h"
#include "offsets.h"
#include <cstdint>

SendChatCommand::SendChatCommand(const char* buffer, size_t buffer_len) {
	if (buffer_len < 1) {
		throw std::invalid_argument("SendChatCommand: buffer too short");
	}
	uint8_t message_len = buffer[0];
	if (message_len != buffer_len - sizeof(message_len)) {
		throw std::invalid_argument("SendChatCommand: wrong buffer length");
	}
	m_message = string(buffer + sizeof(message_len), message_len);
}

void SendChatCommand::operator()() {
	send_chat_func SendChat = reinterpret_cast<send_chat_func>(LolHelper::get_lol_base() + offsets::send_chat_message);
	DWORD message_to_send_addr = LolHelper::get_lol_base() + offsets::send_chat_message_this + offsets::send_chat_message_this_message_offset;
	// maximum of 100 chars because riot puts this max too, probably size of the buffer
	strncpy(reinterpret_cast<char*>(message_to_send_addr), m_message.c_str(), 100);
	
	uint32_t send_chat_this = LolHelper::get_lol_base() + offsets::send_chat_message_this;

	SendChat(send_chat_this);
}