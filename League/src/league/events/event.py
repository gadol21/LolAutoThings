class Event(object):
    """
    an event recieved from the injected dll.
    """

    def __init__(self, msg):
        """
        inheriting classes should implement.
        initializes an event object from message buffer.
        :param msg: the buffer of the recieved message, to be parsed
        """
        raise NotImplementedError('inheriting objects should implement this')
