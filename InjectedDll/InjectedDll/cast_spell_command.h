#ifndef __CAST_SPELL_COMMAND_H
#define __CAST_SPELL_COMMAND_H

#include "command.h"
#include <cstdint>

typedef struct {
	float x;
	float z;
	float y;
} position;

/**
 * spellmanager consists of 6 pointers to the 6 spells
 * (0=Q;1=W;2=E;3=R;4=D;5=F)
 */
typedef struct {
	void* spells[6];
} spellmanager;

/// this function is __thiscall. make sure to pass ecx explicitly before calling it
typedef void(__stdcall * cast_spell_func)(void* spell_information, uint32_t spell_index, position& target, position& source, uint32_t target_unit);

typedef struct {
	uint32_t main_champion;
	uint8_t spell_index;
	position target_pos;
	position source_pos;
	uint32_t target_unit;
} cast_spell_message;

/**
 * Sends a CastSpell command - cast a given spell to a given target. protocol:
 *
 * uint32_t main_champion (our hero)
 * uint8_t spell_index (0=Q;1=W;2=E;3=R;4=D;5=F)
 * position target_pos
 * position source_pos (for most spells it is our pos, or zeros)
 * uint32_t target_unit
 */
class CastSpellCommand : public Command {
public:
	/**
	 * Parses a buffer with a given length.
	 * @throws invalid_argument if buffer is malformatted
	 */
	CastSpellCommand(const char* buffer, size_t buffer_len);

	virtual void operator()();

private:
	uint32_t m_main_champion;
	uint8_t m_spell_index;
	position m_target_pos;
	position m_source_pos;
	uint32_t m_target_unit;
};

#endif // ndef __CAST_SPELL_COMMAND_H