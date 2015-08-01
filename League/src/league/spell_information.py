from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString
from memory_reader import MemoryReader

class SpellInformation(MemoryReader):
    """
    represents a SpellInformation object, contains information about the given spell
    """

    def __init__(self, engine, addr):
        """
        :param addr: address of the SpellInformation
        """
        super(SpellInformation, self).__init__(engine)
        self.addr = addr

    def get_fields(self):
        properties = {
                        'level':(0x10, Int),
                        'stacks':(0x18, Int)
                     }
        properties.update(super(SpellInformation, self).get_fields())
        return properties

    @property
    def cd(self):
        """
        returns the time left until the skill will be up.
        if the skill is already up, 0 is returned
        """
        cd_offset = 0x14
        game_time_offset = 0x11155B0
        spell_online_time = self.read(Float, self.addr + cd_offset)
        game_time = self.read(Float, self._engine.get_module_addr() + game_time_offset)
        time_till_online = spell_online_time - game_time
        return time_till_online if time_till_online > 0 else 0

    @property
    def name(self):
        """
        returns the spell's name
        """
        spelldata_offset = 0xd4
        unknown_offset = 0xc
        name_offset = 0x8
        spelldata = self.read(Int, self.addr + spelldata_offset)
        unknown = self.read(Int, spelldata + unknown_offset)
        return self.read(NullTerminatedString, unknown + name_offset)