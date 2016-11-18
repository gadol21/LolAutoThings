from league.field_types import *
from league_object import LeagueObject
from league.dynamic_vars_manager import DynamicVarsManager
from league.offsets import Offsets


class AttackableUnit(LeagueObject):
    # Map between a type (for example, 'AIHeroClient') to its dynamic fields
    _DYNAMIC_FIELDS = {}

    def __init__(self, engine, addr):
        super(AttackableUnit, self).__init__(engine, addr)

        # A map of {field_name: field_offset}
        self.init_dynamic_fields()

    @property
    def _dynamic_var_manager(self):
        var_manager_addr = self.read(Int, self.addr + Offsets.DYNAMIC_VAR_MANAGER_OFFSET)
        return DynamicVarsManager(self._engine, var_manager_addr)

    def init_dynamic_fields(self):
        my_type = self.type

        if my_type in self._DYNAMIC_FIELDS:
            self._dynamic_fields = self._DYNAMIC_FIELDS[my_type]
            return
        self._dynamic_fields = self._dynamic_var_manager.get_all()
        self._DYNAMIC_FIELDS[my_type] = self._dynamic_fields

    def get_dynamic_fields(self):
        return {'is_invulnrable': ('IsInvulnerable', Bool),
                'armor': ('mArmor', Float),
                'shield': ('mAllShield', Float),
                'move_speed': ('mMoveSpeed', Float),
                'health': ('mHP', Float),
                'max_health': ('mMaxHP', Float)}
