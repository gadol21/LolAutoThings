#pragma once

#include "hooker.h"

/**
 * Installs a hook on the function that removes objects from the object list,
 * and sends back via socket the deleted object's base address
 */
class ObjectRemoveHooker {
public:
	static ObjectRemoveHooker& get_instance();

	/**
	 * Installs the hook on the function that removes objects from the list
	 */
	void install_hook();

private:
	ObjectRemoveHooker();

	friend void __stdcall on_callback_object_remove(DWORD object_to_remove);

	Hooker m_hooker;
};

/**
* this function gets called from callback_object_remove, and contains all of the hook logic
*/
void __stdcall on_callback_object_remove(DWORD object_to_remove);

/**
* This function gets called at the hook
* @note: it must not destroy the stack or change registers,
*		  therefor it is best if it is naked and only calls another function
*		  that cleans its own stack
*/
void callback_object_remove();
