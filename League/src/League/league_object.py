from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString
from objects_handler import ObjectsHandler


class LeagueObject(object):
    """
    Describe abstract class for league object, provide the ability to read
    data of the object.
    """

    BASE_FIELDS = {'name_length': (0x30, Int)}

    def __init__(self, list_index):
        self._readers = {Byte: ObjectsHandler().engine.read_byte, Short: ObjectsHandler().engine.read_short,
                         Int: ObjectsHandler().engine.read_int, Float: ObjectsHandler().engine.read_float,
                         NullTerminatedString: ObjectsHandler().engine.read_string,
                         LengthedString: ObjectsHandler().engine.read_string}
        self.id = list_index
        self.addr = ObjectsHandler().engine.object_addr(list_index)
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
        return self.read(field_type, self.addr + offset, *args)

    def read(self, field_type, addr, *args):
        return self._readers[field_type](addr, *args)


    @property
    def name(self):
        name_pos = 0x20
        if self.name_length < 16:
            return self.read(LengthedString, self.addr + name_pos, self.name_length)
        name_addr = self.read(Int, self.addr + name_pos)
        return self.read(NullTerminatedString, name_addr)

    @property
    def type(self):
        type_struct_offset = 0x4
        len_offset = 0x14
        type_string_offset = 0x4  # inside the struct
        type_struct = self.read(Int, self.addr + type_struct_offset)
        type_len = self.read(Int, type_struct + len_offset)
        if type_len < 16:
            return self.read(LengthedString, type_struct + type_string_offset, type_len)
        string_addr = self.read(Int, type_struct + type_string_offset)
        return self.read(NullTerminatedString, string_addr)

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
        return self.ObjectsHandler().engine.dump_memory(self.addr)