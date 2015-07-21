from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString


class LeagueObject(object):
    """
    Describe abstract class for league object, provide the ability to read
    data of the object.
    """

    BASE_FIELDS = {'name_length': (0x30, Int)}

    def __init__(self, engine, list_index):
        self._engine = engine
        self._readers = {Byte: engine.read_byte, Short: engine.read_short,
                         Int: engine.read_int, Float: engine.read_float,
                         NullTerminatedString: engine.read_string,
                         LengthedString: engine.read_string}
        self._addr = engine.object_addr(list_index)
        self._fields = self.get_fields()
        self._fields.update(self.BASE_FIELDS)

    def __getattr__(self, item):
        if not item in self._fields:
            raise KeyError("Could not find the specific attr")
        field = self._fields[item]
        if len(field) == 2:
            offset, field_type = field
            args = ()
        else:
            offset, field_type, args = field
        return self._readers[field_type](self._addr + offset, *args)

    @property
    def name(self):
        if self.name_length < 16:
            return self._readers[LengthedString](self._addr + 0x20, self.name_length)
        name_addr = self._readers[Int](self._addr + 0x20)
        return self._readers[NullTerminatedString](name_addr)

    @property
    def type(self):
        type_addr = self._readers[Int](self._addr + 0x4)
        type_len = self._readers[Int](type_addr + 0x14)
        if type_len < 16:
            return self._readers[LengthedString](type_addr + 0x4, type_len)
        type_addr = self._readers[Int](type_addr + 0x4)
        return self._readers[NullTerminatedString](type_addr)

    def get_fields(self):
        """
        You should override it
        :return: dictionary - for each property name it contain tuple of
         offset and type.
        {'hp': (10, Int), 'name': (20, NullTerminatedString)}
        The unique one is: {'name': (20, LengthedString, (5))} when 5 is the
        length of the string.
        """
        return {}

    def dump_memory(self):
        return self._engine.dump_memory(self._addr)