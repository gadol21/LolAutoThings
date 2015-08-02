from field_types import *
from memory_reader import MemoryReader
from spell_information import SpellInformation


class SpellManager(MemoryReader):
    """
    represents an object's spell manager,
    contains spell information for q, w, e, r, d, f.
    """
    MANAGER_INDEXES = {'q': 0, 'w': 1, 'e': 2, 'r': 3, 'd': 4, 'f': 5}

    def __init__(self, engine, addr):
        """
        :param addr: the address of the spell manager
        """
        super(SpellManager, self).__init__(engine)
        self.addr = addr

    @property
    def q(self):
        """
        :return: SpellInformation object representing the given spell
        """
        index_in_magager = self.MANAGER_INDEXES['q']
        spellinfo_addr = self.read(Int, self.addr + 4 * index_in_magager, index_in_magager)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def w(self):
        """
        :return: SpellInformation object representing the given spell
        """
        index_in_magager = self.MANAGER_INDEXES['w']
        spellinfo_addr = self.read(Int, self.addr + 4 * index_in_magager, index_in_magager)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def e(self):
        """
        :return: SpellInformation object representing the given spell
        """
        index_in_magager = self.MANAGER_INDEXES['e']
        spellinfo_addr = self.read(Int, self.addr + 4 * index_in_magager, index_in_magager)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def r(self):
        """
        :return: SpellInformation object representing the given spell
        """
        index_in_magager = self.MANAGER_INDEXES['r']
        spellinfo_addr = self.read(Int, self.addr + 4 * index_in_magager, index_in_magager)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def d(self):
        """
        :return: SpellInformation object representing the given spell
        """
        index_in_magager = self.MANAGER_INDEXES['d']
        spellinfo_addr = self.read(Int, self.addr + 4 * index_in_magager, index_in_magager)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def f(self):
        """
        :return: SpellInformation object representing the given spell
        """
        index_in_magager = self.MANAGER_INDEXES['f']
        spellinfo_addr = self.read(Int, self.addr + 4 * index_in_magager, index_in_magager)
        return SpellInformation(self._engine, spellinfo_addr)
