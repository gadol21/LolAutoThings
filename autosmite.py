from league import *
import time


class AutoSmite(object):

	TARGETS = ["SRU_Dragon6.1.1", "SRU_Red4.1.1", "SRU_Red10.1.1", "SRU_Blue1.1.1", "SRU_Blue7.1.1",
	"SRU_Baron12.1.1"]
	SMITE_RANGE = 500
	
	def __init__(self):
		self.me = None
		self.targets = None
		self.last_easy = None
	
	def init(self):
		self.me = get_me()
		if self.me is None:
			raise RuntimeError('Please rerun')
		self.last_easy = time.time()
		self.targets = []
		objs = get(Minion)
		for obj in objs:
			if obj.name in self.TARGETS:
				self.targets.append(obj)
		

	def get_smite_dmg(self):
		dmgs = [390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000]
		return dmgs[self.me.level - 1]
		
	def get_smite_pos(self):
		if "summonersmite" in self.me.spell_manager.d.name:
			return 4
		return 5
		
	def smite_available(self):
		if "summonersmite" in self.me.spell_manager.d.name:
			return self.me.spell_manager.d.cd == 0 and self.me.spell_manager.d.stacks > 0
		if "summonersmite" in self.me.spell_manager.f.name:
			return self.me.spell_manager.f.cd == 0 and self.me.spell_manager.f.stacks > 0
		return False

	def step(self):
		if self.me.health == 0:
			return
		for obj in self.targets:
			if (obj.health <= self.get_smite_dmg() and self.smite_available() and
			(((obj.x - self.me.x) ** 2 + (obj.y - self.me.y) ** 2) ** 0.5) < self.SMITE_RANGE):
				cast_spell(self.me.addr, self.get_smite_pos(), (obj.x, obj.z, obj.y), (0, 0, 0), obj.addr)
				if 'Dragon' in obj.name or 'Baron' in obj.name:
					if time.time() - self.last_easy > 3:
						write_to_chat("easy")
						self.last_easy = time.time()
						
	def on_object_added(self, obj_addr):
		obj = get_obj(obj_addr)
		if obj.name in self.TARGETS:
			self.targets.append(obj)
		
	def on_object_removed(self, obj_addr):
		#check if the object is jungle monster
		for i in xrange(len(self.targets)):
			if self.targets[i].addr == obj_addr:
				self.targets.remove(self.targets[i])
				return 