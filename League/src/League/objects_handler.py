from league import Champion, Minion
from objreader import Engine
from league_object import LeagueObject

LIST_SIZE = 10 ** 4
engine = Engine()

TYPE_CLASSES = {"AIHeroClient": Champion, "obj_AI_Minion": Minion}
DEFAULT_TYPE = LeagueObject


def _get_obj(index):
    obj = LeagueObject(engine, index)
    if obj.type in TYPE_CLASSES:
        return TYPE_CLASSES[obj.type](engine, index)
    return obj


def get(obj_type):
    """
    :param obj_type: Class of the objects to return, for example: Champion will
      return all the champions.
    :return: list contains all the objects of the selected type.
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = _get_obj(i)
            if type(obj) == obj_type:
                objects.append(obj)
    return objects


def get_by_name(name):
    """
    :return: list contain all the objects with the specific name
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = _get_obj(i)
            if obj.name == name:
                objects.append(obj)
    return objects


def get_by_type(obj_type):
    """
    :return: list contain all the objects with the specific type
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            obj = _get_obj(i)
            if obj.type == obj_type:
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