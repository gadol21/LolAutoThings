cheats = []

def add_cheat(cheat):
	cheats.append(cheat)
	
def run():
	while True:
		for cheat in cheats:
			cheat.step()