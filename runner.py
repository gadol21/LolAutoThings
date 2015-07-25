from socket import socket
from select import select
from struct import unpack
PORT = 24766

cheats = []

def add_cheat(cheat):
	cheats.append(cheat)
	
def run():
	s = socket(socket.AF_INET, socket.SOCK_DGRAM)
	s.bind(('127.0.0.1', PORT))
	for cheat in cheats:
		cheat.init()
	while True:
		while len(select([s], [], [], 0)[0]) > 0:
			msg = socket.recv(5)
			if ord(msg[0]) == 0:
				for cheat in cheats:
					cheat.on_object_added(unpack("I", msg[1:])[0])
			elif ord(msg[0]) == 1:
				for cheat in cheats:
					cheat.on_object_removed(unpack("I", msg[1:])[0])
			else:
				raise RuntimeError("INVALID MSG TYPE")
		for cheat in cheats:
			cheat.step()
			

if __name__ == '__main__':
	from autosmite import AutoSmite
	from warddetector import WardDetector
	add_cheat(AutoSmite())
	add_cheat(warddetector())