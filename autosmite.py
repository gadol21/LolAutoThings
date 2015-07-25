from league import *

me = get_me()

TARGETS = ["SRU_Dragon6.1.1", "SRU_Red4.1.1", "SRU_Red10.1.1", "SRU_Blue1.1.1", "SRU_Blue7.1.1"]
SMITE_RANGE = 500

def get_smite_dmg():
	dmgs = [390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000]
	return dmgs[me.level - 1]
	
def get_smite_pos():
	if "summonersmite" in me.spell_manager.d.name:
		return 4
	return 5
	
def smite_available():
	if "summonersmite" in me.spell_manager.d.name:
		return me.spell_manager.d.cd == 0
	if "summonersmite" in me.spell_manager.f.name:
		return me.spell_manager.f.cd == 0
	return False

def step():
	objs = get(Minion)
	for obj in objs:
		if obj.name in TARGETS:
			if (obj.health <= get_smite_dmg() and smite_available() and
			(((obj.x - me.x) ** 2 + (obj.y - me.y) ** 2) ** 0.5) < SMITE_RANGE):
				cast_spell(me.addr, get_smite_pos(), (obj.x, obj.z, obj.y), (0, 0, 0), obj.addr)
				if 'Dragon' in obj.name:
					write_to_chat("easy")
			
if __name__ == '__main__':
	while True:
		step()