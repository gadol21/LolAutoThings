from field_types import Int, Short, Byte, Float, NullTerminatedString, LengthedString
from memory_reader import MemoryReader
from spell_information import SpellInformation

class SpellManager(MemoryReader):
    """
    represents an object's spell manager,
    contains spell information for q, w, e, r, d, f.
    """

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
        q_pos = 0
        spellinfo_addr = self.read(Int, self.addr + q_pos)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def w(self):
        """
        :return: SpellInformation object representing the given spell
        """
        q_pos = 4
        spellinfo_addr = self.read(Int, self.addr + q_pos)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def e(self):
        """
        :return: SpellInformation object representing the given spell
        """
        q_pos = 8
        spellinfo_addr = self.read(Int, self.addr + q_pos)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def r(self):
        """
        :return: SpellInformation object representing the given spell
        """
        q_pos = 0xc
        spellinfo_addr = self.read(Int, self.addr + q_pos)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def d(self):
        """
        :return: SpellInformation object representing the given spell
        """
        q_pos = 0x10
        spellinfo_addr = self.read(Int, self.addr + q_pos)
        return SpellInformation(self._engine, spellinfo_addr)

    @property
    def f(self):
        """
        :return: SpellInformation object representing the given spell
        """
        q_pos = 0x14
        spellinfo_addr = self.read(Int, self.addr + q_pos)
        return SpellInformation(self._engine, spellinfo_addr)