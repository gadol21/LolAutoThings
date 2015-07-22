from socket import socket
from struct import pack
from utils import singleton


@singleton
class FunctionCaller(object):

    PORT = 33782
    FLOATING_TEXT = 1
    SEND_CHAT = 2
    CAST_SPELL = 3
    MOVE_ATTACK = 4

    def _send_command(self, msg_type, args_str):
        s = socket()
        s.connect(('127.0.0.1', self.PORT))
        s.write(pack('I{0}s'.format(len(args_str)), msg_type, args_str))
        s.close()

    def write_to_chat(self, msg):
        self._send_command(self.SEND_CHAT, pack('b{0}s', len(msg), msg))