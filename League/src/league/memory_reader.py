from field_types import *


class MemoryReader(object):
    """
    represents an object that reads its own properties from league's memory.
    inheriting classes should implement get_fields, and these fields
    """

    def __init__(self, engine):
        self._engine = engine
        self._readers = {Bool: MemoryReader.read_bool(engine),
                         Byte: engine.read_byte, Short: engine.read_short,
                         Int: engine.read_int, Float: engine.read_float,
                         NullTerminatedString: engine.read_string,
                         LengthedString: engine.read_string}
        self._fields = self.get_fields()
        # To be initialized by inheriting classes
        self._dynamic_fields = {}

    @staticmethod
    def read_bool(engine):
        def inner(addr):
            return engine.read_byte(addr) != '\x00'
        return inner

    def __getattr__(self, item):
        dynamic_fields = self.get_dynamic_fields()
        if item not in self._fields and \
           item not in dynamic_fields:
            raise KeyError("Could not find the specific attr")

        # Read dynamic field
        if item in dynamic_fields:
            riot_name, field_type = dynamic_fields[item]
            offset = self._dynamic_fields[riot_name]
            return self.read(field_type, self.addr + offset)

        # Read regular field
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
        return {}

    def get_dynamic_fields(self):
        """
        You should override it
        :returns: dictionary - for each property name it contains a tuple for
                               Riot's name for that property, and its type.
                               example: {'hp': ('mHp', Float)}
        """
        return {}

    def dump_memory(self):
        """
        dumps the first X bytes from the object's base address.
        X is defined in ObjReader
        """
        return self._engine.dump_memory(self.addr)

    def __dir__(self):
        """
        allows tab-completion for remote fields
        """
        return sorted(set(dir(type(self)) +
                          self.__dict__.keys() +
                          self.get_fields().keys() +
                          self.get_dynamic_fields().keys()))
