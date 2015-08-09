#ifndef __CHAT_HOOKER_H
#define __CHAT_HOOKER_H

#include "hookers/hooker.h"
#include <string>

using std::string;

/**
* Hooks SendChat.
* sends home messages that start with MS_COMMAND_PREFIX
*/
class ChatHooker {
public:
	static ChatHooker& get_instance();
	
	/// installs the hook
	void install_hook();

	/**
	* every chat message starting with this string is a command intended
	* to be sent home.
	* @note: chat messages starting with this prefix will not be sent (the original SendChat won't be called)
	*/
	static const string MS_COMMAND_PREFIX;
private:
	/// constructor
	ChatHooker();

	Hooker m_hooker;
};

/**
* this function gets called from callback_object_remove, and contains all of the hook logic
* @returns: true if the message is a command and there is no need to call the original SendChat,
*			false if the message is not a command
*/
bool __stdcall on_callback_chat_send(void* this_ptr);

/**
* This function gets called at the hook
* @note: it must not destroy the stack or change registers,
*		 therefor it is best if it is naked and only calls another function
*		 that cleans its own stack
*/
void callback_chat_send();

#endif Source// ndef __CHAT_HOOKER_H