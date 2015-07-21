#include "hooker.h"
#include "offsets.h"
#include <Windows.h>
#include <stdexcept>
#include <algorithm>

DWORD callback_ret_addr = 0;
const char* Hooker::MOV_EDI_EDI = "\x8b\xff";

Hooker::Hooker() : m_is_hooked(false) { }

Hooker::~Hooker() {
	if (m_is_hooked) {
		// 2 bytes is the size of short jmp
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
	DWORD lol_handle = reinterpret_cast<DWORD>(GetModuleHandle(L"League of Legends.exe"));
	if (lol_handle == NULL) {
		throw std::runtime_error("failed to find league exe");
	}
	m_hook_addr = lol_handle + offsets::main_loop;
	callback_ret_addr = m_hook_addr + 2;
	hotpatch(m_hook_addr, reinterpret_cast<DWORD>(callback));
	m_is_hooked = true;
}

void Hooker::hotpatch(DWORD address, DWORD callback) {
	DWORD long_jump_addr = address - 5;
	DWORD old_protection = change_protection(long_jump_addr, PAGE_EXECUTE_READWRITE, 7);
	// E9 is the opcode for relative long jump
	*reinterpret_cast<uint8_t*>(long_jump_addr) = '\xE9';
	*reinterpret_cast<DWORD*>(long_jump_addr + 1) = get_relative_address(long_jump_addr, callback);
	// now write the short jmp, and finish the hook
	// jump 7 bytes back, where the long jump is
	int8_t short_jump = -7;
	// EB is the opcode for relative short jump
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

void __stdcall on_callback() {
	Hooker& hooker = Hooker::get_instance();
	// skip the short jmp (2 bytes)
	//void(*original_main_loop)() = reinterpret_cast<void(*)()>(hooker.m_hook_addr + 2);
	for (list<hooker_callback>::iterator it = hooker.m_callbacks.begin(); it != hooker.m_callbacks.end(); ++it) {
		(*it)();
	}
	//original_main_loop();
}

__declspec(naked) void callback() {
	__asm {
		pushad
		call on_callback
		popad
		jmp callback_ret_addr
	}
}

void Hooker::register_callback(hooker_callback callback) {
	// don't add if the callback already exists
	if (std::find(m_callbacks.begin(), m_callbacks.end(), callback) == m_callbacks.end()) {
		m_callbacks.push_back(callback);
	}
}

void Hooker::remove_callback(hooker_callback callback) {
	m_callbacks.remove(callback);
}