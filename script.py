from league.objreader import Engine
from league.league_object import LeagueObject

e = Engine()
e.print_debug_info()
print "================"

from time import time
me = None
for i in xrange(10000):
	if e.object_exist(i):
		obj = LeagueObject(e, i)
		if obj.name == 'CG Potato':
			me = obj

#del obj
#del e