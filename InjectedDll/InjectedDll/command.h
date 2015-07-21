#ifndef __COMMAND_H
#define __COMMAND_H

#include <list>
#include <vector>
#include <memory>

using std::vector;
using std::unique_ptr;

class Command;
typedef unique_ptr<Command> CommandPtr;

/**
 * This class parses a command and executes it.
 */
class Command {
public:
	/**
	 * gets a command buffer, and parses it. forwards it to the correct command class,
	 * and returns it.
	 * @throws invalid_argument if the buffer is malformatted
	 */
	static CommandPtr Create(const char* command, size_t command_len);

	/**
	 * acts upon the command
	 */
	virtual void act() = 0;
};

#endif //ndef __COMMAND_H