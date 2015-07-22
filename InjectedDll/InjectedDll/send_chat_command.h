#ifndef __SEND_CHAT_COMMAND_H
#define __SEND_CHAT_COMMAND_H

#include "command.h"
#include <string>

using std::string;

typedef void(__stdcall *send_chat_func)(const char* message);

/**
 * Represents a send chat command. protocol (what is expected to be in the buffer):
 *
 * uint8_t message_len
 * char message[message_len]
 */
class SendChatCommand : public Command {
public:
	SendChatCommand(const char* buffer, size_t buffer_len);

	virtual void operator ()();

private:
	string m_message;
};

#endif // ndef __SEND_CHAT_COMMAND_H