#ifndef __SERVER_H
#define __SERVER_H

#include <stdint.h>
#include <Windows.h>

const char* LOOPBACK = "127.0.0.1";

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
	Server(uint16_t port);

	~Server();

	/**
	 * start accepting connections. blocks
	 */
	void start();

private:
	uint16_t m_port;
};

#endif // ndef __SERVER_H