from league.field_types import *
from attackable_unit import AttackableUnit
from league.spell_manager import SpellManager
from league.offsets import Offsets
import league.functions


class Champion(AttackableUnit):
    def __init__(self, engine, addr):
        super(Champion, self).__init__(engine, addr)

    def get_fields(self):
        properties = {'mana': (Offsets.MANA_OFFSET, Float),
                      'max_mana': (Offsets.MAX_MANA_OFFSET, Float)}
        properties.update(super(Champion, self).get_fields())
        return properties

    def get_dynamic_fields(self):
        properties = {'gold': ('mGold', Float),
                      'total_gold': ('mGoldTotal', Float),
                      'cdr': ('mPercentCooldownMod', Float),
                      'attack_range': ('mAttackRange', Float),
                      'crit_chance': ('mCrit', Float),
                      'level': ('mLevelRef', Int)}
        properties.update(super(Champion, self).get_dynamic_fields())
        return properties

    @property
    def spell_manager(self):
        """
        :return: the spell manager associated with this champion
        """
        return SpellManager(self._engine, self.addr + Offsets.SPELL_MANAGER_OFFSET)

    def cast_skillshot(self, spell_info, target_pos, src_pos=None):
        """
        like ezreal q
        :param spell_info: the spell information of the spell (obtained using spell_manager)
        :param target_pos: tuple (x, z, y)
        :param src_pos: for skills like rumble ult or victor e the requires a start pos
        """
        if src_pos is None:
            src_pos = self.position
        league.functions.cast_spell(self.addr, spell_info, target_pos, src_pos, 0)

    def cast_self_spell(self, spell_info):
        """
        spells like hecarim q
        :param spell_info: the spell information of the spell (obtained using spell_manager)
        """
        league.functions.cast_spell(self.addr, spell_info, (0, 0, 0), (self.x, self.z, self.y), 0)

    def cast_target(self, spell_info, target):
        """
        spells like veigar's ult
        :param spell_info: the spell information of the spell (obtained using spell_manager)
        target - LeagueObject of the target
        """
        league.functions.cast_spell(self.addr, spell_info, (target.x, target.z, target.y), (self.x, self.z, self.y), target.netid)

    def auto_attack(self, target):
        """
        auto attacks a target
        :param target: the LeagueObject to attack
        """
        league.functions.attackmove(self.addr, 3, target.position, target.addr, False)

    def move(self, position):
        """
        moves to a target position
        :param position: a tuple of x, z, y
        """
        league.functions.attackmove(self.addr, 2, (1.0 * position[0], 1.0 * position[1], 1.0 * position[2]), 0, False)
