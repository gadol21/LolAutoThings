from league import *
import time


class WardDetector(object):

    def __init__(self):
        self.last = None
        self.wards = None
        
    def init(self):
        self.last = time.time()
        self.wards = [] # tuple of ward and creation time
        current_time = time.time()
        for ward in get_by_name('SightWard'):
            self.wards.append((ward, current_time + ward.ward_time))
        for ward in get_by_name('VisionWard'):
            self.wards.append((ward, current_time + ward.ward_time))

    def step(self):
        current_time = time.time()
        if current_time - self.last > 1:
            for ward, death_time in self.wards:
                ward.floating_text(26, 'Ward ' + str(int(death_time - current_time)))
            self.last = time.time()

    def on_object_added(self, obj_addr):
        obj = get_obj(obj_addr)
        #check if obj is ward
        obj_name = obj.name
        if obj_name == 'SightWard' or obj_name == 'VisionWard':
            self.wards.append((obj, time.time() + obj.ward_time))
        
    def on_object_removed(self, obj_addr):
        #check if anyward in the list got this addr.
        for i in xrange(len(self.wards)):
            if self.wards[i][0].addr == obj_addr:
                self.wards.remove(self.wards[i])
                return