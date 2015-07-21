#ifndef __LOL_HELPER_H
#define __LOL_HELPER_H

#include <Windows.h>

/**
 * a helper class for lol stuff
 */
class LolHelper {
public:
	/**
	 * gets lol module base address.
	 * @throws runtime_error if failed.
	 */
	static DWORD get_lol_base();

private:
	static DWORD ms_lol_base;
};

#endif //ndef __LOL_HELPER_H