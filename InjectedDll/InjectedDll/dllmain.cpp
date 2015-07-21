// dllmain.cpp : Defines the entry point for the DLL application.
#include <Windows.h>
#include <stdexcept>
#include "server.h"
#include "hooker.h"

DWORD __stdcall server_thread(void* params) {
	try{
		Server server(5541);
		while (true) {
			server.handle_one();
		}
	}
	catch (const std::exception& exception) {
		MessageBoxA(NULL, exception.what(), "error", MB_OK);
	}
	return 0;
}

void start_server() {
	try {
		Hooker::get_instance().install_hook();
	}
	catch (const std::runtime_error& exception){
		MessageBoxA(NULL, exception.what(), "error", MB_OK);
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
