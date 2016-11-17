import idc
import idaapi
import idautils
from utils import get_string_list


def _find_attack_move_by_string(string):
    refs = list(idautils.XrefsTo(string.ea))
    assert len(refs) == 1, 'More than one reference to our string!'
    ref = refs[0]

    # This function gets called by attack_move.
    containing_function = idautils.Functions(ref.frm, ref.frm).next()

    refs = list(idautils.XrefsTo(containing_function))
    assert len(refs) == 1, 'More than one reference to our function!'
    ref = refs[0]

    # The function contains the call instruction is attack_move
    return idautils.Functions(ref.frm, ref.frm).next()


def find_attack_move():
    for string in get_string_list():
        if 'ALE-E63471E6' in str(string):
            return _find_attack_move_by_string(string)

    raise Exception('Did not find our string')
