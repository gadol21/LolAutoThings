#include "hooker.h"
#include "offsets.h"
#include "lol_helper.h"
#include <Windows.h>
#include <stdexcept>
#include <algorithm>

DWORD callback_ret_addr = 0;
const char* Hooker::MOV_EDI_EDI = "\x8b\xff";

Hooker::Hooker() : m_is_hooked(false) { }

Hooker::~Hooker() {
	if (m_is_hooked) {
		// 2 bytes is the size of near jmp
		DWORD old = change_protection(m_hook_addr, PAGE_EXECUTE_READWRITE, 2);
		memcpy(reinterpret_cast<void*>(m_hook_addr), MOV_EDI_EDI, 2);
		change_protection(m_hook_addr, old, 2);
	}
}

Hooker& Hooker::get_instance() {
	static Hooker instance;
	return instance;
}

void Hooker::install_hook() {
	if (m_is_hooked) {
		// TODO: return something? throw something?
		return;
	}
	DWORD lol_base = LolHelper::get_lol_base();
	if (lol_base == NULL) {
		throw std::runtime_error("failed to find league exe");
	}
	m_hook_addr = lol_base + offsets::main_loop;
	callback_ret_addr = m_hook_addr + 2;
	hotpatch(m_hook_addr, reinterpret_cast<DWORD>(callback));
	m_is_hooked = true;
}

void Hooker::hotpatch(DWORD address, DWORD callback) {
	DWORD long_jump_addr = address - 5;
	DWORD old_protection = change_protection(long_jump_addr, PAGE_EXECUTE_READWRITE, 7);
	// E9 is the opcode for relative far jump
	*reinterpret_cast<uint8_t*>(long_jump_addr) = '\xE9';
	*reinterpret_cast<DWORD*>(long_jump_addr + 1) = get_relative_address(long_jump_addr, callback);
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

void Hooker::register_callback(CommandPtr callback, bool persistent) {
	if (!persistent) {
		m_onetime_callbacks.push_back(std::move(callback));
	}
	else {
		m_persistent_callbacks.push_back(std::move(callback));
	}
}

void __stdcall on_callback() {
	Hooker& hooker = Hooker::get_instance();

	for (list<CommandPtr>::iterator it = hooker.m_persistent_callbacks.begin(); it != hooker.m_persistent_callbacks.end(); ++it) {
		(**it)();
	}
	// note the difference from the previous loop - this one erases each element from the list after it calls it.
	for (list<CommandPtr>::iterator it = hooker.m_onetime_callbacks.begin(); it != hooker.m_onetime_callbacks.end(); it = hooker.m_onetime_callbacks.erase(it)) {
		(**it)();
	}
}

__declspec(naked) void callback() {
	__asm {
		pushad
		call on_callback
		popad
		jmp callback_ret_addr
	}
}