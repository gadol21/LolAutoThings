#ifndef __SEND_CHAT_COMMAND_H
#define __SEND_CHAT_COMMAND_H

#include "command.h"
#include <string>
#include <cstdint>

using std::string;

///note: this is __thiscall. send ecx explicitly before calling
typedef void(__cdecl *send_chat_func)();

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