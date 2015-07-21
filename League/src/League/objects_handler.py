from objreader import Engine
from league_object import LeagueObject


def singleton(class_):
    instances = {}

    def get_instance(*args, **kwargs):
        if class_ not in instances:
            instances[class_] = class_(*args, **kwargs)
        return instances[class_]
    return get_instance


@singleton
class ObjectsHandler(object):

    LIST_SIZE = 10 ** 4

    def __init__(self):
        self.engine = Engine()

    def get_by_name(self, name, obj_class=LeagueObject):
        """
        :return: list contain all the objects with the specific name
        """
        objects = []
        for i in xrange(self.LIST_SIZE):
            if self.engine.object_exist(i):
                obj = obj_class(self.engine, i)
                if obj.name == name:
                    objects.append(obj)
        return objects

    def names_contain(self, string):
        """
        :param string: the string to look for
        :return: all the objects names contain the specific string
        """
        names = set()
        for i in xrange(self.LIST_SIZE):
            if self.engine.object_exist(i):
                obj = LeagueObject(self.engine, i)
                if string.lower() in obj.name.lower():
                    names.add(obj.name)
        return names

    def get_by_type(self, obj_type, obj_class=LeagueObject):
        """
        :return: list contain all the objects with the specific type
        """
        objects = []
        for i in xrange(self.LIST_SIZE):
            if self.engine.object_exist(i):
                obj = obj_class(self.engine, i)
                if obj.type == obj_type:
                    objects.append(obj)
        return objects

    def types_contain(self, string):
        """
        :param string: the string to look for
        :return: all the objects types contain the specific string
        """
        types = set()
        for i in xrange(self.LIST_SIZE):
            if self.engine.object_exist(i):
                obj = LeagueObject(self.engine, i)
                if string.lower() in obj.type.lower():
                    types.add(obj.type)
        return types