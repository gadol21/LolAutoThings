#ifndef __LIST_EVENTS_NOTIFIER
#define __LIST_EVENTS_NOTIFIER

#include <Windows.h>
#include <cstdint>

#pragma pack(push, 1)
typedef struct {
	uint8_t id;
	DWORD param;
} event_message;
#pragma pack(pop)

class ListEventsNotifier {
public:
	static ListEventsNotifier& get_instance();

	/**
	 * sends a notification about an objects that gets added to the list
	 */
	void notify_object_add(DWORD obj);

	/**
	* sends a notification about an objects that gets remover from the list
	*/
	void notify_object_remove(DWORD obj);

private:
	ListEventsNotifier();

	/**
	 * sends a notification about an event
	 */
	void notify_event(uint8_t id, DWORD param);

	static const uint16_t M_PORT;
	static const uint8_t M_OBJECT_ADD;
	static const uint8_t M_OBJECT_REMOVE;
	SOCKET m_socket;
	sockaddr_in m_dest;
};

#endif // ndef __LIST_EVENTS_NOTIFIER