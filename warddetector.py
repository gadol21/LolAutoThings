from league import *
import time

last = time.time()

def step():
	global last
	if time.time() - last > 1:
		wards = get_by_name('SightWard')
		for ward in wards:
			ward.floating_text(26, 'Ward')
		last = time.time()
	
if __name__ == '__main__':
	while True:
		step()