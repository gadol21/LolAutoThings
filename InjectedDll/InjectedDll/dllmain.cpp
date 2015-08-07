// dllmain.cpp : Defines the entry point for the DLL application.
#include <Windows.h>
#include <stdexcept>
#include "server.h"
#include "hookers/main_loop_hooker.h"
#include "hookers/object_remove_hooker.h"
#include "hookers/object_add_hooker.h"
#include "lol_helper.h"
#include "patcher.h"

const uint16_t PORT = 37882;

DWORD __stdcall server_thread(void* params) {
	try{
		Server server(PORT);
		while (true) {
			server.handle_one();
		}
	}
	catch (const std::exception& exception) {
		MessageBoxA(NULL, exception.what(), "fatal error. quiting", MB_OK);
	}
	return 0;
}

void start_server() {
	try {
		Patcher::install_patches();
		MainLoopHooker::get_instance().install_hook();
		ObjectRemoveHooker::get_instance().install_hook();
		ObjectAddHooker::get_instance().install_hook();
	}
	catch (const std::runtime_error& exception){
		MessageBoxA(NULL, exception.what(), "fatal error. quiting", MB_OK);
	}
	CreateThread(NULL,			// Security attributes
				 0,				// Stack size, default
				 server_thread,	// Start address
				 NULL,			// Parameter
				 0,				// Creation flags
				 NULL);			// Out thread id
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		start_server();
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

