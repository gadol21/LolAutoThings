#pragma once
#include <Windows.h>
#include <string>

class Engine {
public:
	Engine();
	bool is_league_running();
	bool start();

	template <typename T>
	T read(size_t offset);

	std::string read_string(size_t offset);
	std::string read_string(size_t offset, size_t length);
	std::string dump_memory(size_t offset);

private:
	void open_process();
	void find_module_addr();
	size_t get_process_id();

	DWORD m_process_id;
	HANDLE m_process_handle;
	BYTE* m_base_addr;

	static const char* M_WINDOW_NAME;
	static const char* M_PROCESS_NAME;
	static const size_t M_DUMP_MEMORY_SIZE = 4096;
};