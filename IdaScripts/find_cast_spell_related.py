import idaapi
import idautils
import idc
from utils import abs_to_rel

MAX_BACK_INSTRUCTIONS = 10
# Memory reference
OPND_TYPE_MEMORY = 2


def _find_function_by_string(string):
    """
    The function that uses this string is CastSpell. just return it.
    """
    refs = list(idautils.XrefsTo(string.ea))
    assert len(refs) == 1, 'More than one reference to our string!'
    ref = refs[0]

    # The from side of the cross reference - the one referenced the string.
    frm = ref.frm
    return idautils.Functions(frm, frm).next()


def find_cast_spell():
    strings = idautils.Strings()

    for string in strings:
        if 'Client Tried to cast a spell' in str(string):
            return _find_function_by_string(string)

    raise Exception('String not found!')


def find_main_champion(cast_spell):
    """
    Given the address of cast_spell, finds main champion.
    """
    calls = list(idautils.XrefsTo(cast_spell))
    assert len(calls) == 1, 'More than one calls to CastSpell!'

    addr = calls[0].frm
    # Go maximum MAX_BACK_INSTRUCTIONS instructions back
    for _ in xrange(MAX_BACK_INSTRUCTIONS):
        if idc.GetMnem(addr) == 'mov' and \
           idc.GetOpType(addr, 1) == OPND_TYPE_MEMORY:
            return idc.GetOperandValue(addr, 1)

        # Go to previous instruction
        addr = idc.PrevHead(addr)

    raise Exception('Main champion not found!')