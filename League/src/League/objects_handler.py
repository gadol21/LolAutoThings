from objreader import Engine


def singleton(class_):
    instances = {}

    def get_instance(*args, **kwargs):
        if class_ not in instances:
            instances[class_] = class_(*args, **kwargs)
        return instances[class_]
    return get_instance


@singleton
class ObjectsHandler(object):

    LIST_SIZE = 10e4

    def __init__(self):
        self.engine = Engine()

    def get_by_name(self, name):
        """
        :return: list contain all the objects with the specific name
        """
        objects = []
        for i in xrange(self.LIST_SIZE):
            if self.engine.object_exist(i):
                obj = LeagueObject(i)
                if obj.name == 'CG Potato':
                    me = obj

    def get_by_type(self, obj_type):
        """
        :return: list contain all the objects with the specific type
        """