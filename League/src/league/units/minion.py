from league.field_types import Float
from league_object import LeagueObject
from league.offsets import Offsets


class Minion(LeagueObject):
    def __init__(self, engine, addr):
        super(Minion, self).__init__(engine, addr)

    def get_fields(self):
        properties = {'ward_time': (Offsets.WARD_TIME_OFFSET, Float)}
        properties.update(super(Minion, self).get_fields())
        return properties
