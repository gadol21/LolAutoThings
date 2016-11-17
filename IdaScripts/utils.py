import idaapi
import idautils

_strings = idautils.Strings()
def get_string_list():
	return _strings

def abs_to_rel(abs):
	return abs - idaapi.get_imagebase()
