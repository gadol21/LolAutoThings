from league.field_types import *
from league.memory_reader import MemoryReader
from league.offsets import Offsets
import league.functions


class LeagueObject(MemoryReader):
    """
    Describe abstract class for league object, provide the ability to read
    data of the object.
    """

    def __init__(self, engine, addr):
        super(LeagueObject, self).__init__(engine)
        self.addr = addr
        if self.addr == 0:
            raise IndexError("There is no object in this address ")

    def __eq__(self, other):
        if type(other) == int:
            return self.addr == other
        return self.addr == other.addr

    @property
    def id(self):
        """
        the object's id in the list. should be unique
        """
        return self._engine.get_obj_id(self.addr)

    @property
    def name(self):
        """
        the object's name. for champions it is the summoner's name, not the champion's name
        """
        name_pos = 0x20
        if self.name_length < 16:
            return self.read(LengthedString, self.addr + name_pos, self.name_length)
        name_addr = self.read(Int, self.addr + name_pos)
        return self.read(NullTerminatedString, name_addr)

    @property
    def type(self):
        """
        the object's internal type
        """
        type_struct_offset = 0x4
        len_offset = 0x14
        type_string_offset = 0x4  # inside the struct
        type_struct = self.read(Int, self.addr + type_struct_offset)
        type_len = self.read(Int, type_struct + len_offset)
        if type_len < 16:
            return self.read(LengthedString, type_struct + type_string_offset, type_len)
        string_addr = self.read(Int, type_struct + type_string_offset)
        return self.read(NullTerminatedString, string_addr)

    @property
    def position(self):
        """
        returns a tuple (x, z, y) representing the object's current location
        """
        return self.x, self.z, self.y

    def get_fields(self):
        """
        You should override it
        :return: dictionary - for each property name it contain tuple of
         offset and type.
        {'hp': (10, Int), 'name': (20, NullTerminatedString)}
        The unique one is: {'name': (20, LengthedString, (5))} when 5 is the
        length of the string.
        """
        return {'team': (Offsets.TEAM_OFFSET, Int),
                'name_length': (Offsets.NAME_LENGTH_OFFSET, Int),
                'x': (Offsets.X_OFFSET, Float), 'z': (Offsets.Z_OFFSET, Float), 'y': (Offsets.Y_OFFSET, Float),
                'health': (Offsets.HEALTH_OFFSET, Float), 'max_health': (Offsets.MAX_HEALTH_OFFSET, Float),
                'netid': (Offsets.NETID_OFFSET, Int)}

    def dump_memory(self):
        """
        dumps the first X bytes from the object's base address.
        X is defined in ObjReader
        """
        return self._engine.dump_memory(self.addr)

    def distance_from(self, another_obj):
        """
        returns the object's distance from another_obj.
        :param another_obj: object to calculate distance from
        :return: the distance
        """
        return ((self.x - another_obj.x) ** 2 + (self.y - another_obj.y) ** 2) ** 0.5

    def floating_text(self, msg_type, msg):
        """
        writes a message on top of an object
        :param msg_type: should be a value of league's enum,
                         no consts yet, 6 should work for most objects, 26 for wards
        :param msg: the message to write
        """
        league.functions.floating_text(self.addr, msg_type, msg)

    def __repr__(self):
        return '<{0} "{1}" at {2}>'.format(self.__class__.__name__, self.name, hex(int(self.addr)))
