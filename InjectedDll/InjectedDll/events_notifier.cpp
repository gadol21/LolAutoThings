#include "events_notifier.h"

const uint16_t EventsNotifier::M_PORT = 24766;

EventsNotifier& EventsNotifier::get_instance() {
	static EventsNotifier instance;
	return instance;
}

EventsNotifier::EventsNotifier() {
	WSADATA wsaData;
	WSAStartup(MAKEWORD(2, 2), &wsaData);
	m_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	m_dest.sin_family = AF_INET;
	m_dest.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");
	m_dest.sin_port = htons(M_PORT);
}

void EventsNotifier::notify_object_add(DWORD obj) {
	notify_object_event(Events::object_add, obj);
}

void EventsNotifier::notify_object_remove(DWORD obj) {
	notify_object_event(Events::object_remove, obj);
}

void EventsNotifier::notify_object_event(Events event_id, DWORD param) {
	object_event_message message;
	message.id = event_id;
	message.param = param;
	send(&message, sizeof(message));
}

void EventsNotifier::notify_chat_command(const string& command) {
	char* to_send = new char[2 + command.size()];
	to_send[0] = Events::chat_command;
	to_send[1] = command.size();
	strncpy(to_send + 2, command.data(), to_send[1]);
	send(to_send, 2 + command.size());
	delete[] to_send;
}

void EventsNotifier::send(void* buffer, size_t size) {
	sendto(m_socket,
		   static_cast<char*>(buffer),
		   size,
		   0,
		   reinterpret_cast<sockaddr*>(&m_dest),
		   sizeof(m_dest));
}
