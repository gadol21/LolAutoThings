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
    """
    writes the given message to chat.
    :param msg: the message to write. should be no longer than 255 chars.
    """
    _send_command(_SEND_CHAT, pack('b{0}s'.format(len(msg)), len(msg), msg))


def floating_text(unit_addr, msg_type, msg):
    _send_command(_FLOATING_TEXT, pack('IIb{0}s'.format(len(msg)), unit_addr, msg_type, len(msg), msg))


def attackmove(main_champ, attackmove_type, target_pos, target_unit, is_attack_move):
    """
    :param main_champ: address of main champion
    :param attackmove_type: attackmove type. should be one of 3 supported consts
    :param target_pos: a tuple (x, z, y)
    :param target_unit: the base address of the unit to attack, or 0
    :param is_attack_move: indicate an AttackMove. cannot be combined with type stop
    """
    command = pack('IIfffbbxx', main_champ, target_unit, target_pos[0], target_pos[1], target_pos[2],
                                attackmove_type, is_attack_move)
    _send_command(_MOVE_ATTACK, command)


def cast_spell(main_champ, spell, target_pos, source_pos, target_unit):
    """
    :param main_champ: address of main champion
    :param spell: the SpellInformation representing the spell.
    :param target_pos: a tuple (x, z, y)
    :param source_pos: a tuple (x, z, y)
    :param target_unit: the target unit, or 0 if skillshot
    """
    command = pack('IIbffffffI', main_champ,
                                 spell.addr,
                                 spell._index,
                                 target_pos[0], target_pos[1], target_pos[2],
                                 source_pos[0], source_pos[1], source_pos[2],
                                 target_unit)
    _send_command(_CAST_SPELL, command)
