#pragma once

template <typename T>
T Engine::read(size_t offset) {
	char buffer[sizeof(T)];
	SIZE_T bytes_read;
	ReadProcessMemory(m_process_handle, m_base_addr + offset, buffer, sizeof(T), &bytes_read);
	return *reinterpret_cast<T*>(buffer);
}