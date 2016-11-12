#pragma once

#include <list>
#include <vector>
#include <memory>
#include "command.h"

using std::vector;
using std::unique_ptr;

typedef unique_ptr<Command> CommandPtr;

/**
 * This class parses a command and executes it.
 */
class CommandFactory {
public:
	/**
	 * gets a command buffer, and parses it. forwards it to the correct command class,
	 * and returns it.
	 * @throws invalid_argument if the buffer is malformatted
	 */
	static CommandPtr Create(const char* command, size_t command_len);
};
