#pragma once

/**
 * this class is responsible for patching required functions
 * in league.
 */
class Patcher {
public:
	/**
	 * install patches in league's funcitons so they will act how we want
	 */
	static void install_patches();

private:
	/**
	 * Patch floating_text so it will support writing on invisible units
	 */
	static void patch_floating_text();
};
