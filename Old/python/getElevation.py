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


def getElevation(locations, **elvtn_args):
	elvtn_args.update({
		'locations': locations
	})

	url = ELEVATION_BASE_URL + '?' + urllib.urlencode(elvtn_args)
	
	print(url)
	
	response = simplejson.load(urllib.urlopen(url))

	# Create a dictionary for each results[] object
	elevationArray = []

	for resultset in response['results']:
		elevationArray.append(resultset['elevation'])
	
	return elevationArray
	# Create the chart passing the array of elevation data
	#getChart(chartData=elevationArray)	 


if __name__ == '__main__':
		
	print("")
	print("Input top-left and bottom-right corner coordinates.")
	print("")

	# Collect the Latitude/Longitude input string
	# from the user
	maxLat = raw_input('Enter the max latitude --> ').replace(' ','')	
	minLon = raw_input('Enter the min longitude --> ').replace(' ','')
	minLat = raw_input('Enter the min latitude --> ').replace(' ','')
	maxLon = raw_input('Enter the max longitude --> ').replace(' ','')

	if not maxLat:
		maxLat = "59.328344"
	if not minLon:
		minLon = "18.076696"
	if not maxLon:
		maxLon = "18.060903"
	if not minLat:
		minLat = "59.321360"

	xcoords = interpolate(float(maxLat), float(minLat), 8)
	ycoords = interpolate(float(minLon), float(maxLon), 8)
	
	'''	
	pathStr = ""
	for x in xcoords:
		pathStr += (str(x) + "|")

	pathStr = pathStr[:-1]
	#print(pathStr)
	'''
	
	pathStr = ""
	
	for x in xcoords:
		for y in ycoords:
			pathStr += (str(x) + "," + (str(y)) + "|")

	pathStr = pathStr[:-1]
	#print(pathStr)

	elevationArray = getElevation(pathStr)

	for el in elevationArray:
		print(el)
		print("")

