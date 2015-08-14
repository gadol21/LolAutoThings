from event import Event
from struct import unpack


class ObjectAdd(Event):
    """
    an event object sent when an object is added to the object list.

    Attributes:
        address - representing the address of the added object
    """
    ID = 0

    def __init__(self, msg):
        self.address = unpack("I", msg)[0]
