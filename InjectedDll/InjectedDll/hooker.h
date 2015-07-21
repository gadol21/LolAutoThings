#ifndef HOOKER_H
#define HOOKER_H

#include <Windows.h>
#include <list>

using std::list;

typedef void(*hooker_callback)();

/**
 * singleton class that is responsible for hooking functions.
 * it is currently hooking the main_loop function in league.
 *
 * @note: currently can only hook main_loop
 * @note: we take advantage of the fact that league supports hotpatching (idk why)
 */
class Hooker{
public:
	~Hooker();

	static Hooker& get_instance();

	/**
	 * hook main_loop function
	 * @throws runtime_error if failed
	 */
	void install_hook();

	/**
	 * Registers a callback to be called every main_loop's step
	 * @note: the callback must return fast! it blocks the main_loop
	 */
	void register_callback(hooker_callback callback);

	void remove_callback(hooker_callback callback);

private:
	Hooker();

	/**
	 * install a hotpatch at a given address
	 * @throws runtime_error if failed
	 */
	void hotpatch(DWORD address, DWORD callback);

	/**
	 * Change the protection of address - address + size to protection.
	 * returns the old protection.
	 * @throws runtime_error if fails
	 */
	DWORD change_protection(DWORD address, DWORD protection, size_t size);

	/**
	 * get relative address. used to get the address to jmp for a jmp instruction
	 */
	DWORD get_relative_address(DWORD from, DWORD to, size_t instruction_size=5);

	static const char* MOV_EDI_EDI;
	list<hooker_callback> m_callbacks;
	bool m_is_hooked;
	/// the address of the function we hook
	DWORD m_hook_addr;

	friend void __stdcall on_callback();
};

/**
* this function is the callback that gets called every main_loop step.
* it executes our code and than calls the original main_loop
* @note: idk if naked is needed here or in the cpp
*/
void callback();

#endif // ndef HOOKER_H