#include "engine.h"
#include <TlHelp32.h>
#include <iostream>
#include <stdexcept>
#include <memory>

const char* Engine::M_WINDOW_NAME = "League of Legends (TM) Client";
const char* Engine::M_PROCESS_NAME = "League of Legends.exe";

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
	std::unique_ptr<char> buffer(new char[length]);
	DWORD bytes_read;
	ReadProcessMemory(m_process_handle, reinterpret_cast<LPCVOID>(offset),
		buffer.get(), length, &bytes_read);
	if (bytes_read == 0) {
		std::cout << "Reading string with specific length, read 0 bytes" << std::endl;
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


size_t Engine::get_process_id() const {
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

void Engine::load_list_addr() {
	m_list_addr = read<DWORD>(reinterpret_cast<DWORD>(m_base_addr + M_OFFSET_LIST));
}

void Engine::print_debug_info() const {
	std::cout << std::hex;
	std::cout << "League running: " << is_league_running() << std::endl;
	std::cout << "Process id: " << m_process_id << std::endl;
	std::cout << "Base addr: " << reinterpret_cast<DWORD>(m_base_addr) << std::endl;
	std::cout << "List addr: " << m_list_addr << std::endl;
	std::cout << "Handle: " << m_process_handle << std::endl;
}