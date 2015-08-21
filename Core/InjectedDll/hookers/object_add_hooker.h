#ifndef __OBJECT_ADD_HOOKER
#define __OBJECT_ADD_HOOKER

#include "hooker.h"

/**
 * Hooks the function that adds objects to the list
 * @note: in this patch the function is not hotpatchable (no mov edi, edi), so we run over the first instruction
 *		  and call this instruction ourself in the hook
 */
class ObjectAddHooker {
public:
	static ObjectAddHooker& get_instance();

	/**
	* Installs the hook on the function that adds objects from the list
	*/
	void install_hook();

private:
	ObjectAddHooker();

	Hooker m_hooker;
};

/**
* this function gets called from callback_object_add, and contains all of the hook logic
*/
void __stdcall on_callback_object_add(DWORD object_to_add);

/**
* This function gets called at the hook
* @note: it must not destroy the stack or change registers,
*		  therefor it is best if it is naked and only calls another function
*		  that cleans its own stack
*/
void callback_object_add();

#endif // ndef __OBJECT_ADD_HOOKER