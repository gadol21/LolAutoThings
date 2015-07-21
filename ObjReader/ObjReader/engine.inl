#pragma once

template <typename T>
T Engine::read(size_t offset) const {
	char buffer[sizeof(T)];
	DWORD bytes_read;
	if (ReadProcessMemory(m_process_handle, reinterpret_cast<LPCVOID>(offset), buffer,
		sizeof(T), &bytes_read) == 0) {
		std::cout << "Failed reading from memory " << GetLastError() << std::endl;
	}
	return *reinterpret_cast<T*>(buffer);
}