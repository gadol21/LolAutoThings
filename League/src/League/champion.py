from league_object import LeagueObject
from field_types import Float


class Champion(LeagueObject):
    def __init__(self, list_index):
        super(Champion, self).__init__(list_index)

    def get_fields(self):
        return {'x': (0x5c, Float),
                'z': (0x60, Float),
                'y': (0x64, Float)}