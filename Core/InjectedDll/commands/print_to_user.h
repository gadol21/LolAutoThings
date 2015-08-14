#ifndef __PRINT_TO_USER
#define __PRINT_TO_USER

#include "commands/command.h"
#include <cstdint>
#include <string>

using std::string;

typedef void(__thiscall *print_to_user_func)(uint32_t this_ptr, const char* message, uint32_t type);

/**
* Print a chat message to user only (doesn't send it to other players)
*
* protocol:
* uint8_t message_len
* char message[message_len]
*/
class PrintToUserCommand : public Command {
public:
	PrintToUserCommand(const char* buffer, size_t buffer_len);

	virtual void operator()();

private:
	string m_message;
};

#endif