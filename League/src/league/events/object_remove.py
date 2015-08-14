from event import Event
from struct import unpack


class ObjectRemove(Event):
    """
    an event object sent when an object is removed from the object list.

    Attributes:
        address - representing the address of the removed object
    """
    ID = 1

    def __init__(self, msg):
        self.address = unpack("I", msg)[0]
