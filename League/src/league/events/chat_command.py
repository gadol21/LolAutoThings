from event import Event


class ChatCommand(Event):
    """
    an event object representing a chat command that was sent.

    Attributes:
        message - the message the user entered in chat, without the command prefix
    :
    """
    ID = 2

    def __init__(self, msg):
        self.message = msg[1:]
