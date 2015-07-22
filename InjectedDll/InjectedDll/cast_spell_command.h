#ifndef __CAST_SPELL_COMMAND_H
#define __CAST_SPELL_COMMAND_H

#include "command.h"

typedef struct {
	float x;
	float z;
	float y;
} position;

/**
 * Sends a CastSpell command - cast a given spell to a given target. protocol:
 *
 * uint32_t main_champion (our hero)
 * uint8_t spell_index (0=Q;1=W;2=E;3=R;4=D;5=F)
 * position target_pos
 * position our_pos
 * uint32_t target_unit
 */
class CastSpellCommand : public Command {
public:
	CastSpellCommand(const char* buffer, size_t buffer_len);

	virtual void operator()();
};

#endif // ndef __CAST_SPELL_COMMAND_H