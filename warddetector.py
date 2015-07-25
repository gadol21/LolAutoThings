from league import *
import time


class WardDetector(object):

	def __init__(self):
		self.last = None
		self.wards = None
		
	def init(self):
		self.last = time.time()
		self.wards = [] # tuple of ward and creation time

	def step(self):
		if time.time() - self.last > 1:
			self.wards = get_by_name('SightWard')
			for ward in self.wards:
				ward.floating_text(26, 'Ward')
			self.last = time.time()

	def on_object_added(self, obj_addr):
		obj = get_obj(obj_addr)
		#check if obj is ward
		print "ward detector obj added"
		
	def on_object_removed(self, obj_addr):
		#check if anyward in the list got this addr.
		print "ward detector obj removed"