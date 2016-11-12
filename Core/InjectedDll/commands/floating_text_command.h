#pragma once

#include "command.h"
#include <string>
#include <cstdint>

using std::string;

typedef struct {
	uint32_t unit;
	uint32_t type;
	uint8_t message_len;
	char message[256];
} floating_text_message;

typedef void(__cdecl *floating_text_func)(uint32_t magic, uint32_t unit, uint32_t type, const char* message);

/**
 * Represents a FloatingText command.
 * the protocol for sending FloatingText (what is expected to be in the buffer):
 *
 * uint32_t unit - the unit to cast FloatingText on
 * uint32_t type - the FloatingText type
 * uint8_t message_len - the length of the message to write
 * char message[message_len] - the message to display on top of the target unit
 */
class FloatingTextCommand : public Command {
public:
	FloatingTextCommand(const char* buffer, size_t buffer_len);

	/**
	 * a functor to be called as callback when time comes to call the FloatingText function
	 */
	virtual void operator ()();

private:
	uint32_t m_unit;
	uint32_t m_type;
	string m_message;
};
