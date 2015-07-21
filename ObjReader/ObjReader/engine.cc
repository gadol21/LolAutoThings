#include "engine.h"
#include <TlHelp32.h>
#include <iostream>
#include <stdexcept>
#include <memory>

const char* Engine::M_WINDOW_NAME = "League of Legends (TM) Client";
const char* Engine::M_PROCESS_NAME = "League of Legends.exe";

Engine::Engine() : m_process_id(0), m_process_handle(NULL),
	m_base_addr(NULL) { }

bool Engine::is_league_running() {
	return FindWindow(NULL, M_WINDOW_NAME) != NULL;
}

std::string Engine::read_string(size_t offset) {
	size_t counter = 0;
	std::string result;
	unsigned char last_char = static_cast<unsigned char>(255);
	SIZE_T bytes_read;
	while (last_char != 0) {
		ReadProcessMemory(m_process_handle, m_base_addr + offset + counter, &last_char, 1, &bytes_read);
		result += last_char;
	}
	return result;
}

std::string Engine::read_string(size_t offset, size_t length) {
	std::unique_ptr<char> buffer(new char[length]);
	SIZE_T bytes_read;
	ReadProcessMemory(m_process_handle, m_base_addr + offset, buffer.get(), length, &bytes_read);
	std::string result(buffer.get());
	return result;
}

std::string Engine::dump_memory(size_t offset) {
	return read_string(offset, M_DUMP_MEMORY_SIZE);
}

bool Engine::start() {
	if (!is_league_running()) {
		throw std::runtime_error("League not running");
		return false;
	}
	m_process_id = get_process_id();
	open_process();
	find_module_addr();
	return true;
}

void Engine::open_process() {
	m_process_handle = OpenProcess(PROCESS_ALL_ACCESS, false, m_process_id);
	if (m_process_handle == NULL) {
		throw std::runtime_error("Could not open the process");
	}

}

void Engine::find_module_addr() {
	HANDLE ptr = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, m_process_id);
	MODULEENTRY32 module;
	module.dwSize = sizeof(module);
	Module32First(ptr, &module);
	while (strcmp(module.szModule, M_PROCESS_NAME)) {
		std::cout << module.szModule << std::endl;
		if (Module32Next(ptr, &module)) {
			throw std::runtime_error("Could not find the module");
		}
	}
	m_base_addr = module.modBaseAddr;
}


size_t Engine::get_process_id() {
	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
	PROCESSENTRY32 pe32;
	pe32.dwSize = sizeof(pe32);
	Process32First(snapshot, &pe32);
	while (strcmp(pe32.szExeFile, M_PROCESS_NAME)) {
		if (!Process32Next(snapshot, &pe32)) {
			throw std::runtime_error("Could not find the process");
		}
	}
	CloseHandle(snapshot);
	return pe32.th32ProcessID;
}