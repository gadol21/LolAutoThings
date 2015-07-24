from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString
from league_object import LeagueObject
from spell_manager import SpellManager


class Champion(LeagueObject):
    def __init__(self, engine, list_index):
        super(Champion, self).__init__(engine, list_index)

    def get_fields(self):
        properties = {'level': (0x3474, Int)}
        properties.update(super(Champion, self).get_fields())
        return properties

    @property
    def spell_manager(self):
        """
        :return: the spell manager associated with this champion
        """
        spell_manager_offset = 0x27e0
        return SpellManager(self._engine, self.addr + spell_manager_offset)