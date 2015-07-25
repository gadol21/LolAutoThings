from league_object import LeagueObject


class Minion(LeagueObject):
    def __init__(self, engine, addr):
        super(Minion, self).__init__(engine, addr)

    def get_fields(self):
        properties = {}
        properties.update(super(Minion, self).get_fields())
        return properties