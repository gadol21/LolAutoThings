/* this dll was intended to be game independent.
 * i still want it to be, so i put all the lol specific functions in this file
 * that can be ignored in case of using another game
 */
#ifndef LOL_H
#define LOL_H

#include "stdafx.h"

const int MOVETYPE_STOP = 0xA;
const int MOVETYPE_ATTACK = 3;
const int MOVETYPE_MOVE = 2;
const int MOVETYPE_ATTACKMOVE = 0;

typedef struct{
	float x;
	float z;
	float y;
} Position, *LPPOSITION;

void FloatingText(DWORD unitBase, char *string, DWORD messageType);
void MoveTo(LPPOSITION position, int moveType, DWORD myChamp, DWORD targetUnit = NULL);

//offsets here
const DWORD FLOATING_TEXT = 0x5E3CE0;
const DWORD FLOATING_TEXT_MAGIC = 0x119B3E0;
const DWORD MOVE_TO = 0x7D3590;

const DWORD UNIT_POSITION = 0x5C; //where x,z,y are inside unit

#endif