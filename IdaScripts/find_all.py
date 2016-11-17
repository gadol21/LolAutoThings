from find_cast_spell_related import find_cast_spell, find_cast_spell_this, find_main_champion
from find_attack_move import find_attack_move
from find_floating_text import find_floating_text
from utils import abs_to_rel

def print_location(name, addr, convert_to_rel=True):
    if convert_to_rel:
        addr = abs_to_rel(addr)
    print '{0} is at {1}'.format(name, hex(addr))

if __name__ == '__main__':
    attack_move = find_attack_move()
    cast_spell = find_cast_spell()
    cast_spell_this = find_cast_spell_this(cast_spell)
    main_champ = find_main_champion(cast_spell)
    floating_text, floating_text_magic = find_floating_text()

    print_location('AttackMove', attack_move)
    print_location('CastSpell', cast_spell)
    print_location('CastSpell this', cast_spell_this, convert_to_rel=False)
    print_location('Main Champion', main_champ)
    print_location('FloatingText', floating_text)
    print_location('FloatingText magic', floating_text_magic)
