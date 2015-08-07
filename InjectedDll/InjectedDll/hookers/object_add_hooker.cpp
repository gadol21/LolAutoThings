#include "object_add_hooker.h"
#include "events_notifier.h"
#include "lol_helper.h"
#include "offsets.h"
#include <string>

using std::string;

DWORD callback_ret_addr_obj_add;

ObjectAddHooker::ObjectAddHooker()
	: m_hooker(reinterpret_cast<DWORD>(callback_object_add),
			   LolHelper::get_lol_base() + offsets::list_add) {
	callback_ret_addr_obj_add = m_hooker.get_hook_addr() + 2;
}

void ObjectAddHooker::install_hook() {
	m_hooker.install_hook();
}

ObjectAddHooker& ObjectAddHooker::get_instance() {
	static ObjectAddHooker instance;
	return instance;
}

void __stdcall on_callback_object_add(DWORD object_to_add) {
	DWORD type_struct = *reinterpret_cast<DWORD*>(object_to_add + 4);
	DWORD type_len = *reinterpret_cast<DWORD*>(type_struct + 0x14);
	string type;
	if (type_len < 16) {
		type = reinterpret_cast<char*>(type_struct + 0x4);
	}
	else {
		type = *reinterpret_cast<char**>(type_struct + 0x4);
	}

	if (type == "obj_AI_Minion") {
		EventsNotifier::get_instance().notify_object_add(object_to_add);
	}
}

_declspec(naked) void callback_object_add() {
	__asm {
		pushad
		// push [esp + 4 + 8*4] because pushad pushed 8 registers to the stack
		push[esp + 0x24]
		call on_callback_object_add
		popad
		jmp callback_ret_addr_obj_add
	}
}