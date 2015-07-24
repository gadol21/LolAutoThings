#pragma once
#include <Windows.h>
#include <string>

class Engine {
public:

	/// Try to start the engine
	Engine();

	/// Close the handle to the process
	~Engine();

	/// Return if league is currently running - game (not client)
	bool is_league_running() const;

	/**
	 * Start the engine, can be called to restart the engine as well.
	 * Throws runtime error if league not running.
	 */
	bool start();

	/// Read template type, for example char, short, int and float.
	template <typename T>
	T read(size_t offset) const;

	/// Read string until null terminated is reached.
	std::string read_string(size_t offset) const;

	/// Read string of length 'length' - does not stop at null terminated.
	std::string read_string(size_t offset, size_t length) const;

	/// Dump the memory at specific offset, can be used to dump the memory of objects.
	std::string dump_memory(size_t offset) const;

	/// The absoulute address of object in the memory.
	DWORD object_addr(size_t index) const;

	/// Check if there is object at specific pos at the list.
	bool object_exist(size_t index) const;

	/// Print debug information such as the process id, the module addr and the list addr.
	void print_debug_info() const;

	DWORD get_module_addr() const;

private:

	/**
	 * Open the handle to the league of legends process.
	 * Throws runtime error if failed.
	 */
	void open_process();

	/// Set the absulute address of the module.
	void find_module_addr();

	/// Set the absoulte address of the object list.
	void load_list_addr();

	/**
	 * Injceting the dll into league of legends. open_process must be called before this.
	 */
	void inject();

	/**
	 * Return the process id of league of legends.
	 * Throws runtime error if the process does not exist.
	 */
	size_t get_process_id() const;

	/// Return the address of the pointer to the object in the list.
	DWORD object_list_addr(size_t index) const;
	
	DWORD m_process_id;
	BYTE* m_base_addr;
	DWORD m_list_addr;
	HANDLE m_process_handle;

	static const char* M_WINDOW_NAME;
	static const char* M_PROCESS_NAME;
	static const size_t M_DUMP_MEMORY_SIZE = 4096;
	static const size_t M_OFFSET_LIST = 0x2D9A390;
	static const char* M_DLL_NAME;
};

#include "engine.inl"