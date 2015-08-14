import socket
import threading
from select import select
from events import *

listener_thread = None
TARGET_IP = '127.0.0.1'
PORT = 24766

EVENT_MAP = {ObjectAdd.ID: ObjectAdd,
             ObjectRemove.ID: ObjectRemove,
             ChatCommand.ID: ChatCommand}

callbacks = {n: [] for n in EVENT_MAP}


def register_callback(event, callback):
    """
    note: the callback will be called from another thread.
    :param event - event to register to. should be one of the event objects
    :param callback - the function that will be called when the event happens.
                      it will receive that event object as parameter.
    """
    callbacks[event.ID].append(callback)


def unregister_callback(event, callback):
    """
    :param event - event to unregister from. should be one of the event objects
    :param callback - the function to remove from the callbacks list
    """
    callbacks[event.ID].remove(callback)


def listen_for_messages():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((TARGET_IP, PORT))
    # TODO: change the condition here to allow stopping the listener
    while True:
        ready_sockets, _, _ = select([sock], [], [], 1)
        for s in ready_sockets:
            msg = s.recv(512)
            event_object = EVENT_MAP[ord(msg[0])](msg[1:])
            for callback in callbacks[event_object.ID]:
                callback(event_object)


def init():
    """
    initializes a thread for the event listener.
    """
    global listener_thread
    listener_thread = threading.Thread(target=listen_for_messages)
    # set to daemon so when the main thread dies this thread will die too
    listener_thread.daemon = True
    listener_thread.start()
