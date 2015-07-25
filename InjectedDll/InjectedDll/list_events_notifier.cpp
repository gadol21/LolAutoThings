#include "list_events_notifier.h"

const uint16_t ListEventsNotifier::M_PORT = 24766;
const uint8_t ListEventsNotifier::M_OBJECT_ADD = 0;
const uint8_t ListEventsNotifier::M_OBJECT_REMOVE = 1;

ListEventsNotifier& ListEventsNotifier::get_instance() {
	static ListEventsNotifier instance;
	return instance;
}

ListEventsNotifier::ListEventsNotifier() {
	WSADATA wsaData;
	WSAStartup(MAKEWORD(2, 2), &wsaData);
	m_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	m_dest.sin_family = AF_INET;
	m_dest.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");
	m_dest.sin_port = htons(M_PORT);
}

void ListEventsNotifier::notify_object_add(DWORD obj) {
	notify_event(M_OBJECT_ADD, obj);
}

void ListEventsNotifier::notify_object_remove(DWORD obj) {
	notify_event(M_OBJECT_REMOVE, obj);
}

void ListEventsNotifier::notify_event(uint8_t id, DWORD param) {
	event_message message;
	message.id = id;
	message.param = param;
	sendto(m_socket,
		   reinterpret_cast<char*>(&message),
		   sizeof(message),
		   0,
		   reinterpret_cast<sockaddr*>(&m_dest),
		   sizeof(m_dest));
}