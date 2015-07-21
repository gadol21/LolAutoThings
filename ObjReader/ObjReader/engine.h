#pragma once
#include <Windows.h>
#include <string>

class Engine {
public:
	/*
	TODO: Add const the what I can.
	*/

	Engine();
	bool is_league_running() const;
	bool start();

	template <typename T>
	T read(size_t offset) const;

	std::string read_string(size_t offset) const;
	std::string read_string(size_t offset, size_t length) const;
	std::string dump_memory(size_t offset) const;

	bool object_exist(size_t index) const;

	void print_debug_info() const;

private:
	void open_process();
	void find_module_addr();
	void load_list_addr();
	size_t get_process_id() const;

	DWORD m_process_id;
	BYTE* m_base_addr;
	DWORD m_list_addr;
	HANDLE m_process_handle;

	static const char* M_WINDOW_NAME;
	static const char* M_PROCESS_NAME;
	static const size_t M_DUMP_MEMORY_SIZE = 4096;
	static const size_t M_OFFSET_LIST = 0x2E59440;
};

#include "engine.inl"