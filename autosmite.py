from league import *

me = get_by_name('CG Potato')[0]

TARGETS = ["SRU_Dragon6.1.1", "SRU_Red4.1.1", "SRU_Red10.1.1", "SRU_Blue1.1.1", "SRU_Blue7.1.1"]
SMITE_RANGE = 500

def get_smite_dmg():
	return 390
	
def get_smite_pos():
	return 4
	
def smite_available():
	return True

def step():
	objs = get(Minion)
	for obj in objs:
		if obj.name in TARGETS:
			if (obj.health < get_smite_dmg() and smite_available() and
			(((obj.x - me.x) ** 2 + (obj.y - me.y) ** 2) ** 0.5) < SMITE_RANGE):
				cast_spell(me.addr, get_smite_pos(), (obj.x, obj.z, obj.y), (0, 0, 0), obj.addr)
			
if __name__ == '__main__':
	while True:
		step()