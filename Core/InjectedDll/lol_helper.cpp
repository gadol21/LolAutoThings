#include "lol_helper.h"
#include <stdexcept>

DWORD LolHelper::ms_lol_base = NULL;

DWORD LolHelper::get_lol_base() {
	if (ms_lol_base == NULL) {
		ms_lol_base = reinterpret_cast<DWORD>(GetModuleHandle(L"League of Legends.exe"));
		if (ms_lol_base == NULL) {
			throw std::runtime_error("failed to find league exe");
		}
	}
	return ms_lol_base;
}