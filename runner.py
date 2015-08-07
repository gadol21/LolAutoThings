import socket
from select import select
from struct import unpack
from autosmite import AutoSmite
from warddetector import WardDetector

PORT = 24766

cheats = {}

CHEAT_NAMES = {'as': AutoSmite, 'wd': WardDetector}

def add_cheat(cheat):
    cheats[cheat] = True
    
def run():
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.bind(('127.0.0.1', PORT))
    for cheat in cheats:
        cheat.init()
    while True:
        while len(select([s], [], [], 0)[0]) > 0:
            msg = s.recv(512)
            if ord(msg[0]) == 0:
                for cheat in cheats:
                    cheat.on_object_added(unpack("I", msg[1:])[0])
            elif ord(msg[0]) == 1:
                for cheat in cheats:
                    cheat.on_object_removed(unpack("I", msg[1:])[0])
            elif ord(msg[0]) == 2:
                message = msg[2:].split(' ')
                if message[0] == 'on':
                    for cheat in cheats:
                        if type(cheat) == CHEAT_NAMES[message[1]]:
                            print 'turning on', cheat.__class__.__name__
                            cheats[cheat] = True
                if message[0] == 'off':
                    for cheat in cheats:
                        if type(cheat) == CHEAT_NAMES[message[1]]:
                            print 'turning off', cheat.__class__.__name__
                            cheats[cheat] = False
            else:
                raise RuntimeError("INVALID MSG TYPE")
        for cheat in cheats:
            if cheats[cheat] == True:
                cheat.step()
            

if __name__ == '__main__':
    add_cheat(AutoSmite())
    add_cheat(WardDetector())
    run()