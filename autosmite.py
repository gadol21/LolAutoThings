from league import *
import time


class AutoSmite(object):
    TARGETS = ["SRU_Dragon_Air6.3.1", "SRU_Dragon_Fire6.3.1", "SRU_Dragon_Water6.3.1", "SRU_Dragon_Earth6.3.1", "SRU_Dragon_Elder6.3.1",
               "SRU_Red4.1.1", "SRU_Red10.1.1", "SRU_Blue1.1.1", "SRU_Blue7.1.1",
               "SRU_Baron12.1.1"]
    SMITE_NAMES = ['S5_SummonerSmitePlayerGanker', 'SummonerSmite', 'S5_SummonerSmiteDuel']
    SMITE_RANGE = 500

    def __init__(self):
        self.me = None
        self.targets = None
        self.last_easy = None
        self.active = True

    def init(self):
        self.me = get_me()
        if self.me is None:
            raise RuntimeError('Please rerun after loading screen')
        self.last_easy = time.time()
        self.targets = []
        for obj in get(Minion):
            if obj.name in self.TARGETS:
                self.targets.append(obj)

    def get_smite_dmg(self):
        dmgs = [390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000]
        return dmgs[self.me.level - 1]

    def get_smite_spell(self):
        if self.me.spell_manager.d.name in self.SMITE_NAMES:
            return self.me.spell_manager.d
        return self.me.spell_manager.f

    def smite_available(self):
        if self.me.spell_manager.d.name in self.SMITE_NAMES:
            return self.me.spell_manager.d.cd == 0 and self.me.spell_manager.d.stacks > 0
        elif self.me.spell_manager.f.name in self.SMITE_NAMES:
            return self.me.spell_manager.f.cd == 0 and self.me.spell_manager.f.stacks > 0
        return False

    def step(self):
        if self.active:
            if self.me.health == 0:
                return
            for obj in self.targets:
                if obj.health <= self.get_smite_dmg() and self.smite_available() and self.me.distance_from(obj) < self.SMITE_RANGE:
                    self.me.cast_target(self.get_smite_spell(), obj)
                    if 'Dragon' in obj.name or 'Baron' in obj.name:
                        if time.time() - self.last_easy > 3:
                            write_to_chat("easy")
                            self.last_easy = time.time()

    def on_object_add(self, obj_addr):
        obj = get_obj(obj_addr)
        if obj.name in self.TARGETS:
            self.targets.append(obj)

    def on_object_remove(self, obj_addr):
        # check if the object is jungle monster
        for i in xrange(len(self.targets)):
            if self.targets[i].addr == obj_addr:
                self.targets.remove(self.targets[i])
                return

    def on_chat(self, message):
        if message == 'off':
            print_to_user('turning auto smite off')
            self.active = False
        elif message == 'on':
            print_to_user('turning auto smite on')
            self.active = True
        else:
            print_to_user('unknown command')
