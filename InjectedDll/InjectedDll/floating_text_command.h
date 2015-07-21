#ifndef __FLOATING_TEXT_COMMAND
#define __FLOATING_TEXT_COMMAND

#include "command.h"

class FloatingTextCommand : public Command {
public:
	FloatingTextCommand(const char* buffer, size_t buffer_len);

	virtual void act();
};

#endif // ndef __FLOATING_TEXT_COMMAND