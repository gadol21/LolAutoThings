import idaapi
import idautils
import idc
from utils import abs_to_rel, get_string_list

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
    for string in get_string_list():
        if 'Client Tried to cast a spell' in str(string):
            return _find_function_by_string(string)

    raise Exception('String not found!')


def _find_cast_spell_caller(cast_spell):
    calls = list(idautils.XrefsTo(cast_spell))
    assert len(calls) == 1, 'More than one calls to CastSpell!'
    return calls[0].frm


def find_main_champion(cast_spell):
    """
    Given the address of cast_spell, finds main champion.
    """
    addr = _find_cast_spell_caller(cast_spell)
    # Go maximum MAX_BACK_INSTRUCTIONS instructions back
    for _ in xrange(MAX_BACK_INSTRUCTIONS):
        if idc.GetMnem(addr) == 'mov' and \
           idc.GetOpType(addr, 1) == OPND_TYPE_MEMORY:
            return idc.GetOperandValue(addr, 1)

        # Go to previous instruction
        addr = idc.PrevHead(addr)

    raise Exception('Main champion not found!')


def find_cast_spell_this(cast_spell):
    """
    Given cast spell, finds the offset in the main champion of the cast_spell_this
    """
    addr = _find_cast_spell_caller(cast_spell)
    addr = idc.PrevHead(addr)

    assert idc.GetMnem(addr) == 'lea'

    # Parse "lea register, [register + numberh]" to get number
    operand = idc.GetOpnd(addr, 1)
    number = operand[operand.index('+') + 1: operand.index('h')]

    return int(number, 16)
