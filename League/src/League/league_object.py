from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString


class LeagueObject(object):
    """
    Describe abstract class for league object, provide the ability to read
    data of the object.
    """

    BASE_FIELDS = {'team': 0}

    def __init__(self, engine, list_index):
        self._engine = engine
        self._readers = {Byte: engine.read_byte, Short: engine.read_short,
                         Int: engine.read_int, Float: engine.read_float,
                         NullTerminatedString: engine.read_string,
                         LengthedString: engine.read_string}
        self._addr = engine.get_object_addr(list_index)
        self._fields = self.get_fields().update(self.BASE_FIELDS)

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

    def get_fields(self):
        """
        :return: dictionary - for each property name it contain tuple of
         offset and type.
        {'hp': (10, Int), 'name': (20, NullTerminatedString)}
        The unique one is: {'name': (20, LengthedString, (5))} when 5 is the
        length of the string.
        """
        raise NotImplemented()