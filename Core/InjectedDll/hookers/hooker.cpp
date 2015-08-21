#include "hooker.h"
#include "offsets.h"
#include "lol_helper.h"
#include <Windows.h>
#include <stdexcept>
#include <algorithm>

Hooker::Hooker(DWORD callback, DWORD patch_addr) 
	: m_is_hooked(false), m_hook_addr(patch_addr), m_callback(callback) { }

Hooker::~Hooker() {
	if (m_is_hooked) {
		// 2 bytes is the size of near jmp
		DWORD old = change_protection(m_hook_addr, PAGE_EXECUTE_READWRITE, 2);
		memcpy(reinterpret_cast<void*>(m_hook_addr), m_original_bytes, 2);
		change_protection(m_hook_addr, old, 2);
	}
}

void Hooker::install_hook() {
	if (m_is_hooked) {
		// TODO: return something? throw something?
		return;
	}
	hotpatch(m_hook_addr, m_callback);
	m_is_hooked = true;
}

void Hooker::hotpatch(DWORD address, DWORD callback) {
	DWORD long_jump_addr = address - 5;
	DWORD old_protection = change_protection(long_jump_addr, PAGE_EXECUTE_READWRITE, 7);
	// E9 is the opcode for relative far jump
	*reinterpret_cast<uint8_t*>(long_jump_addr) = '\xE9';
	*reinterpret_cast<DWORD*>(long_jump_addr + 1) = get_relative_address(long_jump_addr, callback);
	// back up the first two bytes we overwrite so we can restore it upon destruction
	memcpy(m_original_bytes, reinterpret_cast<void*>(address), 2);
	// now write the short jmp, and finish the hook
	// jump 7 bytes back, where the far jump is
	int8_t short_jump = -7;
	// EB is the opcode for relative near jump
	*reinterpret_cast<uint8_t*>(address) = '\xEB';
	*reinterpret_cast<int8_t*>(address + 1) = short_jump;
	change_protection(long_jump_addr, old_protection, 7);
}

DWORD Hooker::change_protection(DWORD address, DWORD protection, size_t size) {
	DWORD old_protection;
	BOOL ret = VirtualProtect(reinterpret_cast<void*>(address),	// Address
							  7,
							  protection,
							  &old_protection);
	if (ret == 0) {
		throw std::runtime_error("failed to change virtual memory protection");
	}
	return old_protection;
}

DWORD Hooker::get_relative_address(DWORD from, DWORD to, size_t instruction_size) {
	return to - (from + instruction_size);
}

DWORD Hooker::get_hook_addr() {
	return m_hook_addr;
}