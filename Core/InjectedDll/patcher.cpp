#include "patcher.h"
#include <cstdint>
#include "lol_helper.h"
#include "offsets.h"

void Patcher::install_patches() {
	patch_floating_text();
}

void Patcher::patch_floating_text() {
	DWORD old_protection;
	DWORD new_protection;

	uint16_t* rituh_loc = reinterpret_cast<uint16_t*>(LolHelper::get_lol_base() + offsets::floating_text_visability_check);
	VirtualProtect(rituh_loc, sizeof(uint16_t), PAGE_EXECUTE_READWRITE, &old_protection);

	/**
	 * 3c 10   cmp al, 10
	 * this check is replacing test al, al
	 * so that if al is 1 or 0 the zero flag won't turn on.
	 * this check follows a function call to is_visible,
	 * we want it to never be false (zero flag never on)
	 */
	*rituh_loc = 0x103c;

	VirtualProtect(rituh_loc, sizeof(uint16_t), old_protection, &new_protection);
}