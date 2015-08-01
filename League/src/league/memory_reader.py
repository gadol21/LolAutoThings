from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString


class MemoryReader(object):
    """
    represents an object that reads its own properties from league's memory.
    inheriting classes should implement get_fields, and these fields
    """

    def __init__(self, engine):
        self._engine = engine
        self._readers = {Byte: engine.read_byte, Short: engine.read_short,
                         Int: engine.read_int, Float: engine.read_float,
                         NullTerminatedString: engine.read_string,
                         LengthedString: engine.read_string}
        self._fields = self.get_fields()

    def __getattr__(self, item):
        if not item in self._fields:
            raise KeyError("Could not find the specific attr")
        field = self._fields[item]
        if len(field) == 2:
            offset, field_type = field
            args = ()
        else:
            offset, field_type, args = field
        return self.read(field_type, self.addr + offset, *args)

    def read(self, field_type, addr, *args):
        """
        enables property reading easily.
        :param field_type: type of the field to read. see file field_types.py for possible types
        :param addr: address to read
        :param args: arguments for the reader, if needed
        """
        try:
            return self._readers[field_type](addr, *args)
        except Exception, e:
            print "ERROR args:", args
            print "the target was", self.id, self.addr
            print "memory"
            print map(ord, self.dump_memory())
            raise e

    def get_fields(self):
        """
        You should override this
        :return: dictionary - for each property name it contain tuple of
         offset and type.
        {'hp': (10, Int), 'name': (20, NullTerminatedString)}
        The unique one is: {'name': (20, LengthedString, (5))} when 5 is the
        length of the string.
        """
        return { }

    def __dir__(self):
        """
        allows tab-completion for remote fields
        """
        return sorted(set(self.__dict__.keys() + self.get_fields().keys()))