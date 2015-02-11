/* note: this methods are not mine, i used the mothods from here:
 * http://www.unknowncheats.me/wiki/Direct3D:DirectX_9_EndScene_Midfunction_Hook_Example
 */
#include "stdafx.h"
#include "pattern.h"
bool bCompare(const BYTE* pData, const BYTE* bMask, const char* szMask)
{
    for(;*szMask;++szMask,++pData,++bMask)
        if(*szMask=='x' && *pData!=*bMask)   return 0;
    return (*szMask) == NULL;
}
 
DWORD FindPattern(DWORD dwdwAdd,DWORD dwLen,BYTE *bMask,char * szMask)
{
    for(DWORD i=0; i<dwLen; i++)
        if (bCompare((BYTE*)(dwdwAdd+i),bMask,szMask))  return (DWORD)(dwdwAdd+i);
    return 0;
}