#include "main_loop_hooker.h"
#include "lol_helper.h"
#include "offsets.h"
#include <Windows.h>

DWORD callback_ret_addr_main_loop;

MainLoopHooker::MainLoopHooker()
	: m_hooker(reinterpret_cast<DWORD>(callback_main_loop), 
			   LolHelper::get_lol_base() + offsets::main_loop) {
	callback_ret_addr_main_loop = m_hooker.get_hook_addr() + 2;
}

MainLoopHooker& MainLoopHooker::get_instance() {
	static MainLoopHooker instance;
	return instance;
}

void MainLoopHooker::install_hook() {
	m_hooker.install_hook();
}

void MainLoopHooker::register_callback(CommandPtr callback, bool persistent) {
	if (!persistent) {
		m_onetime_callbacks.push_back(std::move(callback));
	}
	else {
		m_persistent_callbacks.push_back(std::move(callback));
	}
}

void __stdcall on_callback_main_loop() {
	MainLoopHooker& hooker = MainLoopHooker::get_instance();

	for (list<CommandPtr>::iterator it = hooker.m_persistent_callbacks.begin(); it != hooker.m_persistent_callbacks.end(); ++it) {
		(**it)();
	}
	// note the difference from the previous loop - this one erases each element from the list after it calls it.
	for (list<CommandPtr>::iterator it = hooker.m_onetime_callbacks.begin(); it != hooker.m_onetime_callbacks.end(); it = hooker.m_onetime_callbacks.erase(it)) {
		(**it)();
	}
}

__declspec(naked) void callback_main_loop() {
	__asm {
		pushad
		call on_callback_main_loop
		popad
		jmp callback_ret_addr_main_loop
	}
}