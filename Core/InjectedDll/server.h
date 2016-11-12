#pragma once

#include <cstdint>
#include <Windows.h>

#pragma comment(lib,"ws2_32.lib")

/**
 * Server will listen for connections, and will be able to handle only one connection at a time.
 * it will have its own protocol to communicating with other processes on the same computer.
 * the processes will be able to send commands, and this dll will call functions in lol
 *
 * @note: the server will listen on the given port in tcp
 * @throws: runtime_error if failed to create socket for some reason
 */
class Server {
public:
	/**
	 * Initializes the socket
	 * @note: the server binds the given port on creation, and not on handle_one()
	 * @throws: runtime_error if failed to initialize server
	 */
	Server(uint16_t port);

	~Server();

	/**
	 * accepts one connection, and handle it. blocks until the connection is over
	 */
	void handle_one();

private:
	uint16_t m_port;
	SOCKET m_server;
	static const char* LOOPBACK;
};
