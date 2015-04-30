#include "stdafx.h"
#include "lol.h"
#include <stdio.h>

DWORD base = 0;
void FloatingText(DWORD unitBase,char *string, DWORD messageType) {
	if(!base)
		base = (DWORD)GetModuleHandleA("League of Legends.exe");
	if(!base) //make sure we got it
		return;
	//DWORD magic = *(DWORD *)(base + FLOATING_TEXT_MAGIC); old magic
	DWORD magic = base + 0x10AA530;
	DWORD  funcaddr = base + FLOATING_TEXT;
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

void MoveTo(LPPOSITION position, int moveType, DWORD myChamp, DWORD targetUnit) {
	if (!base)
		base = (DWORD)GetModuleHandleA("League of Legends.exe");
	if (!base) //make sure we got it
		return;
	DWORD  funcaddr = base + MOVE_TO;

	int isAttackMove = (moveType == MOVETYPE_ATTACKMOVE);
	int isStop = (moveType == MOVETYPE_STOP);
	if (moveType == MOVETYPE_ATTACKMOVE){
		if (targetUnit != NULL)
			moveType = MOVETYPE_ATTACK;
		else
			moveType = MOVETYPE_MOVE;
	}
	position = (targetUnit == NULL) ? position : reinterpret_cast<LPPOSITION>(targetUnit + UNIT_POSITION);
	__asm{
		push isStop //idk if this even right, but it works
		push 0
		push isAttackMove
		push targetUnit
		push position
		push moveType
		mov ecx, myChamp
		call funcaddr
	}
}