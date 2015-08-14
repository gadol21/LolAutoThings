#include "engine.h"
#include <TlHelp32.h>
#include <iostream>
#include <stdexcept>
#include <memory>

const char* Engine::M_WINDOW_NAME = "League of Legends (TM) Client";
const char* Engine::M_PROCESS_NAME = "League of Legends.exe";
const char* Engine::M_DLL_NAME = "InjectedDll.dll";

Engine::Engine() : m_process_id(0), m_list_addr(0), m_process_handle(NULL),
	m_base_addr(NULL) {
	if (!start()) {
		std::cout << "Failed to start the engine" << std::endl;
	}
}

Engine::~Engine() {
	CloseHandle(m_process_handle);
}

bool Engine::is_league_running() const {
	return FindWindow(NULL, M_WINDOW_NAME) != NULL;
}

std::string Engine::read_string(size_t offset) const {
	size_t counter = 0;
	std::string result;
	unsigned char last_char = static_cast<unsigned char>(255);
	SIZE_T bytes_read;
	while (last_char != 0) {
		ReadProcessMemory(m_process_handle, reinterpret_cast<LPCVOID>(offset + counter),
			&last_char, 1, &bytes_read);
		result += last_char;
		counter += 1;
	}
	result.pop_back();
	return result;
}

std::string Engine::read_string(size_t offset, size_t length) const {
	if (length == 0) {
		std::cout << "read string with param length = 0" << std::endl;
		return "";
	}
	std::unique_ptr<char> buffer(new char[length]);
	DWORD bytes_read;
	ReadProcessMemory(m_process_handle, reinterpret_cast<LPCVOID>(offset),
		buffer.get(), length, &bytes_read);
	if (bytes_read == 0) {
		std::cout << "Reading string with specific length, read 0 bytes" << std::endl;
		std::cout << "The offset was " << offset << std::endl;
		std::cout << "Last error " << GetLastError() << std::endl;
		throw std::runtime_error("Read Process Memory failed");
	}
	std::string result(buffer.get(), length);
	return result;
}

std::string Engine::dump_memory(size_t offset) const {
	return read_string(offset, M_DUMP_MEMORY_SIZE);
}

DWORD Engine::object_list_addr(size_t index) const {
	return m_list_addr + index * sizeof(void*);
}

DWORD Engine::object_addr(size_t index) const {
	return read<int>(object_list_addr(index));
}

bool Engine::object_exist(size_t index) const {
	return read<int>(object_list_addr(index)) != 0;
}

bool Engine::start() {
	if (!is_league_running()) {
		throw std::runtime_error("League not running");
		return false;
	}
	m_process_id = get_process_id();
	open_process();
	find_module_addr();
	load_list_addr();
	inject();
	return true;
}

void Engine::open_process() {
	m_process_handle = OpenProcess(PROCESS_ALL_ACCESS, false, m_process_id);
	if (m_process_handle == NULL) {
		throw std::runtime_error("Could not open the process");
	}

}

void Engine::find_module_addr() {
	HANDLE snap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, m_process_id);
	MODULEENTRY32 module;
	module.dwSize = sizeof(module);
	BOOL ret = Module32First(snap, &module);
	while (ret == TRUE) {
		if (strcmp(module.szModule, M_PROCESS_NAME) == 0) {
			m_base_addr = module.modBaseAddr;
			return;
		}
		ret = Module32Next(snap, &module);
	}
	CloseHandle(snap);
	throw std::runtime_error("Could not find lol base");
}


size_t Engine::get_process_id() const {
	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
	PROCESSENTRY32 pe32;
	pe32.dwSize = sizeof(pe32);
	BOOL ret = Process32First(snapshot, &pe32);
	while (ret == TRUE) {
		if (strcmp(pe32.szExeFile, M_PROCESS_NAME) == 0) {
			CloseHandle(snapshot);
			return pe32.th32ProcessID;
		}
		ret = Process32Next(snapshot, &pe32);
	}
	CloseHandle(snapshot);
	throw std::runtime_error("Could not find the process");
}

void Engine::load_list_addr() {
	m_list_addr = read<DWORD>(reinterpret_cast<DWORD>(m_base_addr + M_OFFSET_LIST));
}

void Engine::inject() {
	FARPROC loadlibrary_addr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
	LPVOID located_memory = VirtualAllocEx(m_process_handle, NULL, 1024,
		MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
	SIZE_T bytes_written;
	char this_path[1023];
	DWORD bytes = GetModuleFileName(GetModuleHandle("_objreader.pyd"), this_path, 1023);
	std::string dll_path(this_path);
	dll_path = dll_path.substr(0, dll_path.find_last_of("/\\") + 1);
	dll_path += M_DLL_NAME;
	BOOL result = WriteProcessMemory(m_process_handle, located_memory, dll_path.c_str(),
		dll_path.length(), &bytes_written);
	DWORD thread_id;
	HANDLE thread_handle = CreateRemoteThread(m_process_handle, NULL, 0,
		reinterpret_cast<LPTHREAD_START_ROUTINE>(loadlibrary_addr),
		located_memory, 0, &thread_id);
	WaitForSingleObject(thread_handle, INFINITE);
	CloseHandle(thread_handle);
}

void Engine::print_debug_info() const {
	std::cout << std::hex;
	std::cout << "League running: " << is_league_running() << std::endl;
	std::cout << "Process id: " << m_process_id << std::endl;
	std::cout << "Base addr: " << reinterpret_cast<DWORD>(m_base_addr) << std::endl;
	std::cout << "List addr: " << m_list_addr << std::endl;
	std::cout << "Handle: " << m_process_handle << std::endl;
}

DWORD Engine::get_module_addr() const {
	return reinterpret_cast<DWORD>(m_base_addr);
}

DWORD Engine::get_obj_id(DWORD obj_addr) const {
	for (size_t i = 0; i < M_LIST_SIZE; ++i) {
		if (object_addr(i) == obj_addr) {
			return i;
		}
	}
	return 0;
}