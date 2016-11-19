from league import *
import time

class CloneDetector(object):
    def __init__(self):
        self.last = None
        self.active = True

    def init(self):
        self.last = time.time()
        # FIXME: add if champ.team != me.team
        self.champion_names = [champ.name for champ in get(Champion)]

    def check_for_more_then_one(self, name):
        objs = get_by_name(name)
        if len(objs) > 1:
            for obj in objs:
                if isinstance(obj, Champion):
                    obj.floating_text(26, 'Real one!')

    def step(self):
        if self.active:
            current_time = time.time()
            if current_time - self.last > 1:
                for name in self.champion_names:
                    self.check_for_more_then_one(name)
                self.last = time.time()

    # TODO: use on_object_add and remove, so we won't need to perform a lot of enumerations every step
    def on_object_add(self, obj_addr):
        pass
        
    def on_object_remove(self, obj_addr):
        pass

    def on_chat(self, message):
        if message == 'off':
            print_to_user('turning clone detector off')
            self.active = False
        elif message == 'on':
            print_to_user('turning clone detector on')
            self.active = True
        else:
            print_to_user('unknown command')