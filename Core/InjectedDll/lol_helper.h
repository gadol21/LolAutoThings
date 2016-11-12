#pragma once

#include <Windows.h>

typedef struct {
	float x;
	float z;
	float y;
} position;

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
