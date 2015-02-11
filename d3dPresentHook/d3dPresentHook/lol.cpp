#include "stdafx.h"
#include "lol.h"

DWORD base = 0;
void FloatingText(DWORD unitBase,char *string, DWORD messageType){
	if(!base)
		base = (DWORD)GetModuleHandleA("League of Legends.exe");
	if(!base) //make sure we got it
		return;
	DWORD magic = *(DWORD *)(base+0x1504D2C);
	DWORD  funcaddr = base + 0x6A4770;
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