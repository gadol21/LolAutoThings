from field_types import *
from memory_reader import MemoryReader


class VarDictionary(MemoryReader):
    """
    Represents a single dictionary that maps variable names to offsets.
    defined as such:
    struct VarDictionary {
        uint32 count;
        uint32 unknown;
        uint32 keys[32];
        uint32 values[32];
    };
    """
    def __init__(self, engine, addr):
        """
        :param addr: the address of the spell manager
        """
        super(VarDictionary, self).__init__(engine)
        self.addr = addr

    def get_fields(self):
        properties = {'count': (0, Int)}
        properties.update(super(VarDictionary, self).get_fields())
        return properties

    def _get_key(self, index):
        assert 0 <= index <= 31
        return self.read(Int, self.addr + 8 + 4*index)

    def _get_value(self, index):
        assert 0 <= index <= 31
        string_ptr = self.read(Int, self.addr + 8 + 4*32 + 4*index)
        return self.read(NullTerminatedString, string_ptr)

    def get(self, index):
        """
        Returns the key-value in the given index
        """
        # Reverse the key and value, so the name is the key and the offset is the value
        return self._get_value(index), self._get_key(index)

    def get_all(self):
        return dict([self.get(i) for i in xrange(self.count)])


class DynamicVarsManager(MemoryReader):
    """
    represents an object's dynamic variable manager.
    Every object have one, and it has offsets to its fields.
    It has 6 VarDictionary objects, each one contains mapping for up to
    32 variables.
    defined as such:
    struct DynamicVarsManager {
        void* vtable;
        VarDictionary dictionaries[6];
    };
    """
    def __init__(self, engine, addr):
        """
        :param addr: the address of the spell manager
        """
        super(DynamicVarsManager, self).__init__(engine)
        self.addr = addr

    def _get_dictionary(self, index):
        assert 0 <= index <= 5
        dictionary_addr = self.read(Int, self.addr + 4 + 4 * index)
        return VarDictionary(self._engine, dictionary_addr)

    @property
    def dictionaries(self):
        return [self._get_dictionary(i) for i in xrange(6)]

    def get_all(self):
        d = {}
        for dictionary in self.dictionaries:
            d.update(dictionary.get_all())
        return d
