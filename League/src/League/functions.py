from socket import socket
from struct import pack

_PORT = 37882
_FLOATING_TEXT = 1
_SEND_CHAT = 2
_CAST_SPELL = 3
_MOVE_ATTACK = 4


def _send_command(msg_type, args_str):
    s = socket()
    s.connect(('127.0.0.1', _PORT))
    s.send(pack('I{0}s'.format(len(args_str)), msg_type, args_str))
    s.close()


def write_to_chat(msg):
    _send_command(_SEND_CHAT, pack('b{0}s'.format(len(msg)), len(msg), msg))


def floating_text(unit_addr, msg_type, msg):
    _send_command(_FLOATING_TEXT, pack('IIb{0}s'.format(len(msg)), unit_addr, msg_type, len(msg), msg))