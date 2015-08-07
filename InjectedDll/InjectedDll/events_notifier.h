#ifndef __LIST_EVENTS_NOTIFIER
#define __LIST_EVENTS_NOTIFIER

#include <Windows.h>
#include <cstdint>
#include <string>

using std::string;

enum Events : uint8_t {
	object_add = 0,
	object_remove = 1,
	chat_command = 2
};

#pragma pack(push, 1)
typedef struct {
	uint8_t id;
	DWORD param;
} object_event_message;
#pragma pack(pop)

class EventsNotifier {
public:
	static EventsNotifier& get_instance();

	/**
	 * sends a notification about an objects that gets added to the list
	 */
	void notify_object_add(DWORD obj);

	/**
	* sends a notification about an objects that gets remover from the list
	*/
	void notify_object_remove(DWORD obj);

	/**
	 * sends a notification about a chat command.
	 * the message is just sent as a buffer of characters
	 */
	void notify_chat_command(const string& command);

private:
	EventsNotifier();

	/// sends a buffer with a given size home via the socket
	void send(void* buffer, size_t size);

	/**
	 * sends a notification about an event
	 */
	void notify_object_event(Events event_id, DWORD param);

	static const uint16_t M_PORT;
	SOCKET m_socket;
	sockaddr_in m_dest;
};

#endif // ndef __LIST_EVENTS_NOTIFIER