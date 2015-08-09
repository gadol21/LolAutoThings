#include "chat_hooker.h"
#include "lol_helper.h"
#include "offsets.h"
#include "events_notifier.h"

const string ChatHooker::MS_COMMAND_PREFIX = "!lol ";

DWORD callback_ret_addr_chat_send;

ChatHooker::ChatHooker() : m_hooker(reinterpret_cast<DWORD>(callback_chat_send),
									LolHelper::get_lol_base() + offsets::send_chat_message) {
	callback_ret_addr_chat_send = m_hooker.get_hook_addr() + 2;
	// Intentionally left empty
}

ChatHooker& ChatHooker::get_instance() {
	static ChatHooker instance;
	return instance;
}

void ChatHooker::install_hook() {
	m_hooker.install_hook();
}

bool __stdcall on_callback_chat_send(void* this_ptr) {
	const char* message = static_cast<char*>(this_ptr) + offsets::send_chat_message_this_message_offset;
	const string& command_prefix = ChatHooker::MS_COMMAND_PREFIX;
	if (strncmp(message, command_prefix.c_str(), command_prefix.size()) == 0) {
		// this is the maximum length for a message. don't send commands over the maximum length
		if (strlen(message) < 0x80) {
			EventsNotifier::get_instance().notify_chat_command(message + command_prefix.size());
		}
		// even if the message wasn't sent, we still don't want it to be printed, it is a command.
		return true;
	}
	return false;
}

_declspec(naked) void callback_chat_send() {
	__asm {
		pushad
		push ecx
		call on_callback_chat_send
		cmp al, 1
		je already_handled_message
		popad
		jmp callback_ret_addr_chat_send

	already_handled_message:
		popad
		ret
	}
}