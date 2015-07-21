#include "server.h"
#include "command_factory.h"
#include "hooker.h"
#include <stdexcept>

const char* Server::LOOPBACK = "127.0.0.1";

Server::Server(uint16_t port) : m_port(port), m_server(NULL) {
	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (result != 0) {
		throw std::runtime_error("Failed to start up WSA.");
	}

	m_server = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (m_server == INVALID_SOCKET) {
		throw std::runtime_error("Failed to create socket");
	}

	sockaddr_in addr = { 0 };
	addr.sin_family = AF_INET;
	addr.sin_addr.S_un.S_addr = inet_addr(LOOPBACK);
	addr.sin_port = htons(port);
	result = bind(m_server, (sockaddr *)&addr, sizeof(addr));
	if (result != 0) {
		throw std::runtime_error("bind failed. port already in use?");
	}
}

Server::~Server() {
	if (m_server != NULL) {
		closesocket(m_server);
	}
}

void Server::handle_one() {
	SOCKET client = accept(m_server,	// Socket
						   NULL,		// Out addr
						   0);			// Out addrlen
	char buffer[1024];
	int bytes_read = 1;
	// 0 is returned from recv if the connection was closed
	while (bytes_read != 0) {
		bytes_read = recv(client,	// Socket
						  buffer,	// buffer
						  1024,		// buffer length
						  0);		// flags

		CommandPtr command(CommandFactory::Create(buffer, bytes_read));
		Hooker::get_instance().register_callback(std::move(command), false);
		// command will get deleted here, probably before the callback was called
	}
	closesocket(client);
}