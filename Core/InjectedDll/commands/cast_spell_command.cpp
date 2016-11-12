#include "cast_spell_command.h"
#include "lol_helper.h"
#include "offsets.h"
#include <stdexcept>

CastSpellCommand::CastSpellCommand(const char* buffer, size_t buffer_len) {
	if (buffer_len != sizeof(cast_spell_message)) {
		throw std::invalid_argument("CastSpell: wrong buffer length");
	}
	const cast_spell_message* message = reinterpret_cast<const cast_spell_message*>(buffer);
	m_main_champion = message->main_champion;
	m_spell_information = message->spell_information;
	m_spell_index = message->spell_index;
	m_source_pos = message->source_pos;
	m_target_pos = message->target_pos;
	m_target_unit = message->target_unit;
}

void CastSpellCommand::operator()() {
	cast_spell_func CastSpell = reinterpret_cast<cast_spell_func>(LolHelper::get_lol_base() + offsets::cast_spell);

	DWORD cast_spell_this = m_main_champion + offsets::cast_spell_this;

	CastSpell(cast_spell_this,
			  m_spell_information,					// SpellInformation
			  m_spell_index,						// spell index
			  m_target_pos,							// target pos
			  m_source_pos,							// source pos
			  m_target_unit);						// target unit
}