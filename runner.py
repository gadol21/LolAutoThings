from league import *
from autosmite import AutoSmite
from clonedetector import CloneDetector
from warddetector import WardDetector

PORT = 24766

cheats = []

CHEAT_NAMES = {'as': AutoSmite,
               'wd': WardDetector,
               'cd': CloneDetector}


def add_cheat(cheat):
    cheats.append(cheat)


def init_cheats():
    for cheat in cheats:
        cheat.init()


def run():
    while True:
        for cheat in cheats:
            cheat.step()


def on_global_chat(message):
    """
    handles a chat command that is not related to a specific cheat
    :param message: the global message
    """
    message = message.split(' ', 1)
    if message[0] == 'modules':
        for shortname, classtype in CHEAT_NAMES.items():
            print_to_user(shortname + ' - ' + classtype.__name__)


def on_chat(event):
    msg = event.message.split(' ')
    if not msg[0] in CHEAT_NAMES:
        on_global_chat(event.message)
        return
    for cheat in cheats:
        if type(cheat) == CHEAT_NAMES[msg[0]]:
            cheat.on_chat(''.join(msg[1:]))


def on_object_add(event):
    for cheat in cheats:
        cheat.on_object_add(event.address)


def on_object_remove(event):
    for cheat in cheats:
        cheat.on_object_remove(event.address)


if __name__ == '__main__':
    init()
    add_cheat(AutoSmite())
    add_cheat(CloneDetector())
    add_cheat(WardDetector())
    init_cheats()
    register_callback(events.ChatCommand, on_chat)
    register_callback(events.ObjectAdd, on_object_add)
    register_callback(events.ObjectRemove, on_object_remove)
    run()
