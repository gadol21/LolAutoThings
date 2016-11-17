import idc
import idaapi
import idautils
from utils import get_string_list

SEARCH_INSTRUCTION_COUNT = 15


def find_call(addr_from, max_inst, call_number=1):
    """
    find the call_number call from addr_from.
    for example, if call_number is 3, finds the third call apearing from addr_from.
    searches over a maximum of max_inst instructions
    """
    for _ in xrange(max_inst):
        if idc.GetMnem(addr_from) == 'call':
            call_number -= 1
            if call_number == 0:
                return addr_from

        addr_from = idc.NextHead(addr_from)

    raise Exception('Did not found the requested call!')



def _find_floating_text_by_string(string):
    refs = list(idautils.XrefsTo(string.ea))
    assert len(refs) == 1, 'More than one reference to our string!'
    ref = refs[0]

    # Look ahead of this string reference, there should be:
    # 1. non interesting call
    # 2. push main_champion. we found it elsewhere, so not interesting aswell
    # 3. push floating_text_magic
    # 4. call floating_text
    # Just find the second call, and its previous instruction.
    second_call_addr = find_call(ref.frm, SEARCH_INSTRUCTION_COUNT, 2)
    floating_text = idc.GetOperandValue(second_call_addr, 0)

    addr = idc.PrevHead(second_call_addr)
    assert idc.GetMnem(addr) == 'push'
    floating_text_magic = idc.GetOperandValue(addr, 0)

    return floating_text, floating_text_magic


def find_floating_text():
    """
    Returns (floating_text, floating_text_magic)
    """
    for string in get_string_list():
        if 'game_floatingtext_quest_received' in str(string):
            return _find_floating_text_by_string(string)

    raise Exception('Did not found our string')
