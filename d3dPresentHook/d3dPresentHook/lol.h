/* this dll was intended to be game independent.
 * i still want it to be, so i put all the lol specific functions in this file
 * that can be ignored in case of using another game
 */
#include "stdafx.h"
#include <Windows.h>
#ifndef LOL_H
#define LOL_H

void FloatingText(DWORD unitBase,char *string,DWORD messageType);

#endif