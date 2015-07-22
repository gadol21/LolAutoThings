from objreader import Engine
from league_object import LeagueObject

LIST_SIZE = 10 ** 4
engine = Engine()


def get_by_name(name, obj_class=LeagueObject):
    """
    :return: list contain all the objects with the specific name
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = obj_class(engine, i)
            if obj.name == name:
                objects.append(obj)
    return objects


def names_contain(string):
    """
    :param string: the string to look for
    :return: all the objects names contain the specific string
    """
    names = set()
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = LeagueObject(engine, i)
            if string.lower() in obj.name.lower():
                names.add(obj.name)
    return names


def get_by_type(obj_type, obj_class=LeagueObject):
    """
    :return: list contain all the objects with the specific type
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = obj_class(engine, i)
            if obj.type == obj_type:
                objects.append(obj)
    return objects


def types_contain(string):
    """
    :param string: the string to look for
    :return: all the objects types contain the specific string
    """
    types = set()
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = LeagueObject(engine, i)
            if string.lower() in obj.type.lower():
                types.add(obj.type)
    return types