#pragma once

#include <stdexcept>
#include <string>

template <typename T>
T Engine::read(size_t offset) const {
	char buffer[sizeof(T)];
	DWORD bytes_read;
	if (ReadProcessMemory(m_process_handle, reinterpret_cast<LPCVOID>(offset), buffer,
						  sizeof(T), &bytes_read) == 0) {
		throw std::runtime_error("Failed reading from memory " + std::to_string(GetLastError()));
	}
	return *reinterpret_cast<T*>(buffer);
}