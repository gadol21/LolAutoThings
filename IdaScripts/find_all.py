from find_cast_spell_related import find_cast_spell, find_main_champion
from utils import abs_to_rel

def print_location(name, abs_addr):
    print '{0} is at {1}'.format(name, hex(abs_to_rel(abs_addr)))

if __name__ == '__main__':
    cast_spell = find_cast_spell()
    main_champ = find_main_champion(cast_spell)

    print_location('CastSpell', cast_spell)
    print_location('Main Champion', main_champ)
