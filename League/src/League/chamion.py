from league_object import LeagueObject
from field_types import Float


class Champion(LeagueObject):
    def __init__(self, engine):
        super(Champion, self).__init__(engine)

    def get_fields(self):
        return {'hp': (Float, 0x20)}