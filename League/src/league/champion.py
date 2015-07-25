from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString
from league_object import LeagueObject
from spell_manager import SpellManager
import functions


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

    def cast_skillshot(self, skill_id, target_pos, src_pos=None):
        """
        like ezreal q
        :param skill_id: the skill id - 0-q, 1-w, 2-e, 3-r, 4-d, 5-f
        :param target_pos: tuple (x, z, y)
        :param src_pos: for skills like rumble ult or victor e the requires a start pos
        """
        if src_pos is None:
            src_pos = self.position
        functions.cast_spell(self.addr, skill_id, target_pos, src_pos, 0)

    def cast_self_spell(self, skill_id):
        """
        spells like hecarim q
        :param skill_id: the skill id - 0-q, 1-w, 2-e, 3-r, 4-d, 5-f
        """
        functions.cast_spell(self.addr, skill_id, 0, (self.x, self.z, self.y), 0)

    def cast_target(self, skill_id, target):
        """
        spells like veigar's ult
        :param skill_id: the skill id - 0-q, 1-w, 2-e, 3-r, 4-d, 5-f
        target - LeagueObject of the target
        """
        functions.cast_spell(self.addr, skill_id, (target.x, target.z, target.y), (self.x, self.z, self.y), target.addr)