#include "hookers/object_remove_hooker.h"
#include "events_notifier.h"
#include "lol_helper.h"
#include "offsets.h"
#include <string>

using std::string;

DWORD callback_ret_addr_obj_remove;

ObjectRemoveHooker::ObjectRemoveHooker() 
	: m_hooker(reinterpret_cast<DWORD>(callback_object_remove),
			   LolHelper::get_lol_base() + offsets::list_remove) {
    // Skip the "push ebp; mov ebp, esp" that we overrun
	callback_ret_addr_obj_remove = m_hooker.get_hook_addr() + 3;
}

ObjectRemoveHooker& ObjectRemoveHooker::get_instance() {
	static ObjectRemoveHooker instance;
	return instance;
}

void ObjectRemoveHooker::install_hook() {
	m_hooker.install_hook();
}

void __stdcall on_callback_object_remove(DWORD object_to_remove) {
	DWORD type_struct = *reinterpret_cast<DWORD*>(object_to_remove + 4);
	DWORD type_len = *reinterpret_cast<DWORD*>(type_struct + 0x14);
	string type;
	if (type_len < 16) {
		type = reinterpret_cast<char*>(type_struct + 0x4);
	}
	else {
		type = *reinterpret_cast<char**>(type_struct + 0x4);
	}
	if (type == "obj_AI_Minion") {
		EventsNotifier::get_instance().notify_object_remove(object_to_remove);
	}
}

__declspec(naked) void callback_object_remove() {
	__asm {
		pushad
		// push [esp + 4 + 8*4] because pushad pushed 8 registers to the stack
		push [esp + 0x24]
		call on_callback_object_remove
		popad

        // The instructions we overrun
        push ebp
        mov ebp, esp
		jmp callback_ret_addr_obj_remove
	}
}