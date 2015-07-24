from league import *
import time

while True:
	wards = get_by_name('SightWard')
	for ward in wards:
		ward.floating_text(6, 'Ward')
	time.sleep(1)