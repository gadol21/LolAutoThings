#include "attackmove_command.h"
#include "lol_helper.h"
#include "offsets.h"
#include <stdexcept>

AttackMoveCommand::AttackMoveCommand(const char* buffer, size_t buffer_len) {
	if (buffer_len != sizeof(attackmove_message)) {
		throw std::invalid_argument("AttackMoveCommand: wrong buffer length");
	}
	const attackmove_message* message = reinterpret_cast<const attackmove_message*>(buffer);

	m_main_champ = message->main_champ;
	m_type = message->type;
	m_target_pos = message->target_pos;
	m_is_attack_move = message->is_attack_move;
}

void AttackMoveCommand::operator()() {
	attackmove_func AttackMove = reinterpret_cast<attackmove_func>(LolHelper::get_lol_base() + offsets::attackmove);

	AttackMove(m_main_champ, m_type, m_target_pos, m_is_attack_move, 0, 0);
}