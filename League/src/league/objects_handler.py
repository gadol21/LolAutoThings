from league.units import *
from objreader import Engine
from functions import print_to_user
import event_listener

LIST_SIZE = 10 ** 4
engine = None

TYPE_CLASSES = {"AIHeroClient": Champion, "obj_AI_Minion": Minion}


def init():
    """
    initializes the engine. must be called first.
    """
    global engine
    engine = Engine()
    event_listener.init()
    print_to_user('LolAutoThings: initialized')


def get_obj(addr):
    obj = LeagueObject(engine, addr)
    if obj.type in TYPE_CLASSES:
        return TYPE_CLASSES[obj.type](engine, addr)
    return obj


def _get_obj_by_index(index):
    return get_obj(engine.object_addr(index))


def get_me():
    """
    :return: the main Champion
    """
    me_offset = 0x1194778
    me = engine.read_int(engine.get_module_addr() + me_offset)
    champions = get(Champion)
    for champ in champions:
        if champ.addr == me:
            return champ


def get(obj_type):
    """
    :param obj_type: Class of the objects to return, for example: Champion will
      return all the champions.
    :return: list contains all the objects of the selected type.
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            try:
                obj = _get_obj_by_index(i)
                if type(obj) == obj_type:
                    objects.append(obj)
            except IndexError:
                pass
    return objects


def get_by_name(name):
    """
    :return: list contain all the objects with the specific name
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            try:
                obj = _get_obj_by_index(i)
                if obj.name == name:
                    objects.append(obj)
            except IndexError:
                pass
    return objects


def get_by_type(obj_type):
    """
    :return: list contain all the objects with the specific type
    """
    objects = []
    for i in xrange(LIST_SIZE):
        if engine.object_exist(i):
            try:
                obj = _get_obj_by_index(i)
                if obj.type == obj_type:
                    objects.append(obj)
            except IndexError:
                pass
    return objects


def names_contain(string):
    """
    :param string: the string to look for
    :return: all the objects names contain the specific string
    """
    names = set()
    for i in xrange(LIST_SIZE):
        try:
            if engine.object_exist(i):
                obj = LeagueObject(engine, engine.object_addr(i))
                if string.lower() in obj.name.lower():
                    names.add(obj.name)
        except IndexError:
                pass
    return names


def types_contain(string):
    """
    :param string: the string to look for
    :return: all the objects types contain the specific string
    """
    types = set()
    for i in xrange(LIST_SIZE):
        try:
            if engine.object_exist(i):
                obj = LeagueObject(engine, engine.object_addr(i))
                if string.lower() in obj.type.lower():
                    types.add(obj.type)
        except IndexError:
                pass
    return types
