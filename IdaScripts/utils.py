import idaapi

def abs_to_rel(abs):
	return abs - idaapi.get_imagebase()
