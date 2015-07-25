#ifndef __MAIN_LOOP_HOOKER_H
#define __MAIN_LOOP_HOOKER_H

#include "command_factory.h"
#include "hooker.h"
#include <list>

using std::list;

class MainLoopHooker {
public:
	static MainLoopHooker& get_instance();

	/**
	 * Installs the hook at the main_loop function
	 */
	void install_hook();

	/**
	* Registers a callback to be called every main_loop's step
	* if persistent = false, the callback will be removed in the next
	* @note: the callback must return fast! it blocks the main_loop
	*/
	void register_callback(CommandPtr callback, bool persistent);

private:
	MainLoopHooker();

	Hooker m_hooker;
	list<CommandPtr> m_onetime_callbacks;
	list<CommandPtr> m_persistent_callbacks;

	friend void __stdcall on_callback_main_loop();
};

/**
 * this function gets called from callback_main_loop, and contains all of the hook logic
 */
void __stdcall on_callback_main_loop();

/**
 * This function gets called at the hook
 * @note: it must not destroy the stack or change registers,
 *		  therefor it is best if it is naked and only calls another function
 *		  that cleans its own stack
 */
void callback_main_loop();

#endif // ndef __MAIN_LOOP_HOOKER_H