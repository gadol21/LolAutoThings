from league import *
import time


class WardDetector(object):

    def __init__(self):
        self.last = None
        self.wards = None
        self.active = True

    def init(self):
        self.last = time.time()
        self.wards = [] # tuple of ward, ward time first time seen, and first time seen
        for ward in get_by_name('SightWard'):
            self.wards.append((ward, ward.ward_time, time.time()))
        for ward in get_by_name('VisionWard'):
            self.wards.append((ward, ward.ward_time, time.time()))

    def step(self):
        if self.active:
            current_time = time.time()
            if current_time - self.last > 1:
                for ward, ward_initial_time, ward_first_seen in self.wards:
                    time_passed = time.time() - ward_first_seen
                    ward.floating_text(26, 'Ward ' + str(int(ward_initial_time - time_passed)))
                self.last = time.time()

    def on_object_add(self, obj_addr):
        obj = get_obj(obj_addr)
        # check if obj is ward
        obj_name = obj.name
        if obj_name == 'SightWard' or obj_name == 'VisionWard':
            self.wards.append((obj, obj.ward_time, time.time()))
        
    def on_object_remove(self, obj_addr):
        # check if anyward in the list got this addr.
        for i in xrange(len(self.wards)):
            if self.wards[i][0].addr == obj_addr:
                self.wards.remove(self.wards[i])
                return

    def on_chat(self, message):
        if message == 'off':
            print_to_user('turning ward detector off')
            self.active = False
        elif message == 'on':
            print_to_user('turning ward detector on')
            self.active = True
        else:
            print_to_user('unknown command')
