#ifndef __ATTACKMOVE_COMMAND
#define __ATTACKMOVE_COMMAND

#include <cstdint>
#include "command.h"
#include "lol_helper.h"

typedef void(__thiscall * attackmove_func)(uint32_t thisptr, uint32_t type, position& target_pos, uint32_t target_unit, uint32_t is_attack_move, uint32_t unknown1, uint32_t unknown2);

typedef struct {
	uint32_t main_champ;
	uint32_t target_unit;
	position target_pos;
	uint8_t type;
	uint8_t is_attack_move;
} attackmove_message;

/**
 * Represents an Attack or Move command (league has one function that handles both)
 * protocol for an AttackMove message:
 *
 * uint32_t main_champ base - the base addr of the main champion object
 * uint8_t type. see AttackMoveCommand's consts M_TYPE_*
 * position target_pos
 * uint32_t target_unit - the unit to attack, or 0
 * uint8_t is_attack_move - bool that indicates this is attackmove. can come with M_TYPE_MOVE and M_TYPE_ATTACK only.
 */
class AttackMoveCommand : public Command {
public:
	/**
	 * parses an AttackMove message
	 * @throws invalid_argument if buffer is malformatted
	 */
	AttackMoveCommand(const char* buffer, size_t buffer_len);

	/**
	 * Performs the AttackMove command
	 */
	virtual void operator()();

private:
	uint32_t m_main_champ;
	uint8_t m_type;
	position m_target_pos;
	uint32_t m_target_unit;
	uint8_t m_is_attack_move;

	/// the AttackMove types supported
	static const uint32_t M_TYPE_MOVE;
	static const uint32_t M_TYPE_ATTACK;
	static const uint32_t M_TYPE_STOP;
};

#endif // ndef __ATTACKMOVE_COMMAND