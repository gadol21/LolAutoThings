from field_types import *
from memory_reader import MemoryReader
from offsets import Offsets


class SpellInformation(MemoryReader):
    """
    represents a SpellInformation object, contains information about the given spell
    """

    def __init__(self, engine, addr, index):
        """
        :param addr: address of the SpellInformation
        :param index: index of the spell in spell manager
        """
        super(SpellInformation, self).__init__(engine)
        self.addr = addr
        self._index = index

    def get_fields(self):
        properties = {'level': (0x10, Int),
                      'stacks': (0x18, Int)}
        properties.update(super(SpellInformation, self).get_fields())
        return properties

    @property
    def cd(self):
        """
        returns the time left until the skill will be up.
        if the skill is already up, 0 is returned
        """
        cd_offset = 0x14
        spell_online_time = self.read(Float, self.addr + cd_offset)
        game_time = self.read(Float, self._engine.get_module_addr() + Offsets.GAME_CLOCK_OFFSET)
        time_till_online = spell_online_time - game_time
        return time_till_online if time_till_online > 0 else 0

    @property
    def name(self):
        """
        returns the spell's name
        """
        unknown_offset = 0xc
        name_offset = 0x8
        spelldata = self.read(Int, self.addr + Offsets.SPELL_DATA_OFFSET)
        unknown = self.read(Int, spelldata + unknown_offset)
        return self.read(NullTerminatedString, unknown + name_offset)

    def __repr__(self):
        return '<{0} "{1}" at {2}>'.format(self.__class__.__name__, self.name, hex(int(self.addr)))
