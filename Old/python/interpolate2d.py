# Copyright Google Inc. 2010 All Rights Reserved
import simplejson
import urllib
from array import array

ELEVATION_BASE_URL = 'http://maps.googleapis.com/maps/api/elevation/json'
CHART_BASE_URL = 'http://chart.googleapis.com/chart'



def interpolate(start, end, nr=16):
	result = []
	if nr < 2:
		return;
	result.append(start)
	step = (end-start)/(nr-1)
	off = start
	for i in range(1, nr-1):
		off += step
		result.append(off)
	result.append(end)
	return result



if __name__ == '__main__':

	# Collect the Latitude/Longitude input string
	# from the user
	
	inp = raw_input("").replace(' ','')
	dim = int(inp)
	
	for i in range
	


