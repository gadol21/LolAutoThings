// dllmain.cpp : Defines the entry point for the DLL application.
#define LEAGUEOFLEGENDS //remove this define, and the dll won't use lol specific stuff
#ifdef _DEBUG
#define LOGFILENAME "dllLog.txt"
#else
#define LOGFILENAME "nul" //nul is the windows equivalent to /dev/null - write to nowhere
#endif

#include <list>
#include <Windows.h>
#include <stdio.h>
#include <d3d9.h>
#include <d3dx9.h>
#include <ctime>
#include <fstream>
#include <string>
#include <TlHelp32.h>

#include "stdafx.h"
#include "pattern.h"
#include "fonts.h"

#ifdef LEAGUEOFLEGENDS
#include "lol.h"
#endif

#pragma comment(lib,"d3dx9.lib")
#pragma comment(lib,"ws2_32.lib")

typedef struct{
	char alpha,red,green,blue;
}color;

#define WAIT_INFINITE -10000

typedef struct{
	int fontSize;
	int timeleft;
	int format;
	RECT rect;
	color clr;
	char text[128];
}text;

clock_t lastTime=0; //maybe replace this, cpu time instead of real time
SOCKET server;
BYTE presentOriginalBytes[5],resetOriginalBytes[5];
BYTE presentHookedBytes[5],resetHookedBytes[5];
DWORD present,reset,oldProtection;
std::list<text> texts;
HANDLE textsMutex;
std::ofstream logfile;

typedef IDirect3D9* (_stdcall *D3DCREATE9)(UINT); //function ptr to Direct3DCreate9

void myPaint(IDirect3DDevice9 *dev){
	logfile << "myPaint begin" << std::endl;
	logfile.flush();
	clock_t current = clock();
	if(!lastTime)
		lastTime = current;
	WaitForSingleObject(textsMutex,INFINITE); //make sure only we use texts
	if(FAILED(dev->BeginScene()))
		return;
	std::list<std::list<text>::iterator> toRemove;
	for(std::list<text>::iterator it = texts.begin();it!=texts.end();it++){
		ID3DXFont *font = Fonts::GetFont((*it).fontSize,dev);
		if(font){
			//calculate the required rect needed
			if((*it).rect.bottom == -1 && (*it).rect.right == -1){
				RECT rect = {0}; //manually do the left center and right thing
				font->DrawTextA(NULL,(*it).text,-1,&rect,DT_CALCRECT, D3DCOLOR_ARGB((*it).clr.alpha ,(*it).clr.red ,(*it).clr.green ,(*it).clr.blue));
				if((*it).format == DT_LEFT){
					rect.left += (*it).rect.left;
					rect.right += (*it).rect.left;
				}else if((*it).format == DT_CENTER){
					int width = rect.right - rect.left;
					rect.left += (*it).rect.left - width/2;
					rect.right += (*it).rect.left - width/2;
				}else if((*it).format == DT_RIGHT){
					int width = rect.right - rect.left;
					rect.left += (*it).rect.left - width;
					rect.right += (*it).rect.left - width;
				}else{ //not a valid format
					continue;
				}
				rect.top += (*it).rect.top;
				rect.bottom += (*it).rect.top;
				(*it).rect = rect;
			}
			unsigned int val = font->DrawTextA(NULL,(*it).text,-1,&(*it).rect,(*it).format, D3DCOLOR_ARGB((*it).clr.alpha ,(*it).clr.red ,(*it).clr.green ,(*it).clr.blue));
			if ((*it).timeleft != WAIT_INFINITE){
				(*it).timeleft -= current - lastTime;
				if ((*it).timeleft < 0)
					toRemove.push_back(it);
			}
		}
	}
	//now remove what need to be deleted
	for(std::list<std::list<text>::iterator>::iterator it = toRemove.begin();it!=toRemove.end();it++){
		texts.erase(*it);
	}
	dev->EndScene();
	lastTime = current;
	ReleaseMutex(textsMutex); //finish using texts
	logfile << "myPaint end" << std::endl;
	logfile.flush();
}

HRESULT _stdcall HookedReset(IDirect3DDevice9 *self,D3DPRESENT_PARAMETERS *params){
	logfile << "reset hook" << std::endl;
	logfile.flush();
	WaitForSingleObject(textsMutex,INFINITE);
	//release the font we use with this device before resetting in, or it will fail (why?!)
	Fonts::Reset();

	memcpy((void *)reset,resetOriginalBytes,5); //restore the first 5 bytes (remove the hook)
	HRESULT returnval = self->Reset(params); //now call the real present function.
	memcpy((void *)reset,resetHookedBytes,5); //hook back
	ReleaseMutex(textsMutex);
	return returnval;
}

//present belongs to IDirect3DDevice9, therefor its first parameter is this, but this is a reserved word so i will use self.
HRESULT _stdcall HookedPresent9(IDirect3DDevice9 *self,const RECT *pSourceRect, const RECT *pDestRect, HWND hDestWindowOverride, const RGNDATA *pDirtyRegion){
	logfile << "present hook" << std::endl;
	logfile.flush();
	//we need to call our draw, restore the first 5 bytes of present, call it, and hook it again.
	myPaint(self);

	memcpy((void *)present,presentOriginalBytes,5); //restore the first 5 bytes (remove the hook)
	HRESULT returnval = self->Present(pSourceRect,pDestRect,hDestWindowOverride,pDirtyRegion); //now call the real present function.
	memcpy((void *)present,presentHookedBytes,5); //hook back
	return returnval;
}

/*
 * we need to create an IDirect3DDevice9.
 * because it is an interface, first four bytes will be a pointer to the vtable,
 * 17th functions there is Present, and we'll load its address.
 * we'll create IDirect3D9, and use its function CreateDevice to obtain the IDirect3DDevice9
 */
void LoadD3D9Functions(){
	HMODULE hD3d9 = LoadLibraryA("d3d9");

	DWORD *vtable;// = *(reinterpret_cast<DWORD **>(d3dDevice));
	DWORD PPPDevice = FindPattern((DWORD)hD3d9, 0x128000, (PBYTE)"\xC7\x06\x00\x00\x00\x00\x89\x86\x00\x00\x00\x00\x89\x86", "xx????xx????xx");
    memcpy( &vtable, (void *)(PPPDevice + 2), 4);

	//we are now going to unload hD3d9, so the address of present may change. therefor, we will take the relative address
	present = vtable[17]-(DWORD)hD3d9;
	reset = vtable[16] - (DWORD)hD3d9;
	//now release everything we used
	FreeLibrary(hD3d9);
}

DWORD _stdcall MakeDemHookz(LPVOID lpParam){
	LoadD3D9Functions();
	HMODULE hD3d9 = GetModuleHandleA("d3d9.dll");
	//hook present
	present = present+(DWORD)hD3d9; //now load the real address into present
	VirtualProtect((void *)present,8,PAGE_EXECUTE_READWRITE,&oldProtection);
	memcpy(presentOriginalBytes,(void *)present,5);
	presentHookedBytes[0] = 0xE9; //opcode for jmp
	DWORD jmpAddr = (DWORD)HookedPresent9 - (present+5); //relative address = target address - current address. current address is the address after the jmp statement (5 bytes)
	memcpy(presentHookedBytes+1,&jmpAddr,sizeof(DWORD));
	memcpy((void *)present,presentHookedBytes,5);
	//hook reset - we need to hook it in order to release any device specific things we use before the reset happens
	reset = reset+(DWORD)hD3d9;
	VirtualProtect((void *)reset,8,PAGE_EXECUTE_READWRITE,&oldProtection);
	memcpy(resetOriginalBytes,(void *)reset,5);
	resetHookedBytes[0] = 0xE9; //opcode for jmp
	jmpAddr = (DWORD)HookedReset - (reset+5); //relative address = target address - current address. current address is the address after the jmp statement (5 bytes)
	memcpy(resetHookedBytes+1,&jmpAddr,sizeof(DWORD));
	memcpy((void *)reset,resetHookedBytes,5);

	//after the hook is created, init a socket to communicate with comjector
	WSADATA wsaData;
	WSAStartup(MAKEWORD(2, 2), &wsaData);
	server = socket(AF_INET,SOCK_DGRAM,IPPROTO_UDP);
	sockaddr_in addr;
	addr.sin_family = AF_INET;
	addr.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");
	addr.sin_port = htons(2501); //host to network short
	bind(server,(sockaddr *)&addr,sizeof(addr));
	//listen(server,5); no need - udp
	while(1){
		char buffer[256];
		recv(server,buffer,256,0);
		if(memcmp(buffer,"text",4)==0){
			text txt = {0};
			txt.timeleft = *(int*)(buffer+4);
			txt.fontSize = *(int*)(buffer+8);
			int x = *(int*)(buffer+12);
			int y = *(int*)(buffer+16);
			txt.rect.top = y;
			txt.rect.left = x;
			txt.rect.bottom = -1;
			txt.rect.right = -1;
			txt.clr.alpha = buffer[20]; //copy color, argb
			txt.clr.red = buffer[21];
			txt.clr.green = buffer[22];
			txt.clr.blue = buffer[23];
			txt.format = *(int *)(buffer+24);
			strcpy(txt.text,buffer+28);
			WaitForSingleObject(textsMutex,INFINITE); //make sure only we use texts
			texts.push_back(txt);
			ReleaseMutex(textsMutex); //finish using texts.
		}else if(memcmp(buffer,"remove",6)==0){
			WaitForSingleObject(textsMutex,INFINITE); //make sure only we use texts
			std::list<text>::iterator it = texts.begin();
			for(;it!=texts.end();it++){
				if(strcmp(buffer+6,(*it).text)==0){
					break;
				}
			}			
			if(it!=texts.end())
				texts.erase(it);
			ReleaseMutex(textsMutex); //finish using texts.
		}else if(memcmp(buffer,"clear",5)==0){
			WaitForSingleObject(textsMutex,INFINITE); //make sure only we use texts
			texts.clear();
			ReleaseMutex(textsMutex); //finish using texts
		}
#ifdef LEAGUEOFLEGENDS
		else if(memcmp(buffer,"floating",8)==0){
			//DWORD unitBase
			//DWORD messageType
			//char[] message
			DWORD unitBase = *(DWORD*)(buffer+8);
			DWORD messageType = *(DWORD*)(buffer+12);
			char message[128];
			strcpy(message,buffer+16);
			logfile << "calling FloatingText with params: " << unitBase<< "," << messageType << "," << message << std::endl;
			logfile.flush();
			FloatingText(unitBase,message,messageType);
			logfile << "FloatingText returned" << std::endl;
		logfile.flush();
		}
#endif
	}
	return 0;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved){
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		logfile.open(LOGFILENAME,std::ios_base::out);
		logfile << "attached" << std::endl;
		logfile.flush();
		textsMutex = CreateMutex(NULL,FALSE,NULL);
		CreateThread(NULL,0,MakeDemHookz,NULL,0,NULL);
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		logfile.close();
		break;
	}
	return TRUE;
}

