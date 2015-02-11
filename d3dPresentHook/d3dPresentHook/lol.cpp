#include "stdafx.h"
#include "lol.h"
#include <stdio.h>

DWORD base = 0;
void FloatingText(DWORD unitBase,char *string, DWORD messageType){
	if(!base)
		base = (DWORD)GetModuleHandleA("League of Legends.exe");
	if(!base) //make sure we got it
		return;
	DWORD magic = *(DWORD *)(base + 0x1DF821C);
	DWORD  funcaddr = base + 0x8CAD00;
	__asm{
		push esi //for some reason this func changes esi, and this causes problems
		push string
		push messageType
		push unitBase
		push magic
		call dword ptr [funcaddr]
		add esp, 0x10
		pop esi
	}
}