import sys      #for main()
import argparse #for parsing args

import urllib2
import json

#for getting the google image
import Image
import StringIO

class ImageData(object):
    
    @property
    def center(self):
        """(x,y) - two doubles"""
        return self._center
        
    @property
    def size(self):
        """(width,height) - two integers"""
        return self._size
        
    @property
    def zoom(self):
        """zoom - an integer"""
        return self._zoom
        
    @property
    def is_center(self):
        """If this location is given by center."""
        return len(self.center) == 2
        
    def __init__(self,center,size,zoom):
        self._center = center
        self._size = size
        self._zoom = zoom
        
class BingImageMetaData(object):
    @property
    def bounding_box(self):
        """Bounding box is tuple of size 4 with the bound coordinates in following order:
        
        South West North East."""
        return self._bb
        
    @property
    def image_info(self):
        return self._id
        
    @property
    def center(self):
        return self._id.center
        
    @property
    def size(self):
        return self._id.size
        
    @property
    def zoom(self):
        return self._id.zoom
        
    def __init__(self,bounding_box,image_data):
        """Bounding_box should be a tuple of len 4, image_info is an ImageData object."""
        self._bb = bounding_box
        self._id = image_data
        
    
    
class BinaryImageData(object):
    
    @property
    def metadata(self):
        """Returns the original BingImageMetaData this object was constructed from."""
        return self._md
        
    @property
    def bounding_box(self):
        return self._md.bounding_box
        
    @property
    def center(self):
        return self._md.center
        
    @property
    def size(self):
        return self._md.size
        
    @property
    def zoom(self):
        return self._md.zoom
        
    @property
    def binary_img(self):
        """An Image object with mode '1'."""
        return self._bimg
    
    def __init__(self,metadata,binary_img):
        self._md = metadata
        self._bimg = binary_img

################
# FUNCTIONS
################       
def construct_binary_image(location,size,zoom,bing_key,google_key=None):    
    ii = ImageData(location,size,zoom)
    metadata = get_bing_image_meta_data(ii,bing_key)
    binary_image_data = get_binary_image_info(metadata)
    return binary_image_data

def get_bing_image_meta_data(image_info,bing_key):
    """Calls Bing static map API and gets some metadata info about a the given ImageData.
    
    The metadata is returned as a BingImageMetaData object."""
    #example:
    #https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/59.3256954,18.071867/13?mapMetadata=1&o=xml&key=bing_key
    #http://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial?mapArea=59.1,17.9,59.5,18.3&key=bing_key
    url = "https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial"
    if image_info.is_center:
        center = "/" + str(image_info.center[0]) + "," + str(image_info.center[1]) + "/"
        url += center
        url += str(image_info.zoom)
        size = str(image_info.size[0]) + "," + str(image_info.size[1])
        url += "?mapSize=" + size + "&mapMetadata=1&key=" + bing_key
    else:
        url += "?mapArea="+str(image_info.center[0]) + "," + str(image_info.center[1]) + \
               "," + str(image_info.center[2]) + "," + str(image_info.center[3])
        url += "&mapSize=" + str(image_info.size[0]) + "," + str(image_info.size[1])
        url += "&mapMetadata=1&key=" + bing_key
    
    #the url is now constructed, send request
    f = urllib2.urlopen(url).read()
    #the answer is in a specific JSON format
    obj = json.loads(f)
    resource = obj["resourceSets"][0]["resources"][0]
    #resource is now a dictionary with the relevant metadata.
    
    bounding_box = tuple(resource["bbox"])
    size = (int(resource["imageHeight"]),int(resource["imageWidth"]))
    center = tuple([float(coord) for coord in resource["mapCenter"]["coordinates"]])
    zoom = int(resource["zoom"])
    id = ImageData(center,size,zoom)
    bimd = BingImageMetaData(bounding_box,id)
    return bimd
    
    
def get_binary_image_info(metadata,google_key=None):
    """Calls Google Static Map API with the given image metadata, returns a BinaryImageData.
    
    google_key not implemented yet"""
    #example
    #https://maps.googleapis.com/maps/api/staticmap?center=59.3256954,18.0718675&zoom=13&size=400x400&style=feature:transit|visibility:off&style=feature:road|visibility:off&style=feature:poi|visibility:off&style=feature:administrative|visibility:off&style=feature:landscape|visibility:off&style=feature:water|element:labels|visibility:off
    url = "https://maps.googleapis.com/maps/api/staticmap?center="
    url += str(metadata.center[0]) + "," + str(metadata.center[1])
    url += "&zoom=" + str(metadata.zoom)
    url += "&size=" + str(metadata.size[0]) + "x" + str(metadata.size[1])
    
    #add style - strip map of everything
    #remove transits and roads
    url += "&style=feature:transit|visibility:off&style=feature:road|visibility:off"
    #remove poi and administrative
    url += "&style=feature:poi|visibility:off&style=feature:administrative|visibility:off"
    #remove landscapes
    url += "&style=feature:landscape|visibility:off"
    #remove water labels
    url += "&style=feature:water|element:labels|visibility:off"
    #set water color
    url += "&style=feature:water|hue:A9CAFD"
    f = StringIO.StringIO(urllib2.urlopen(url).read())
    img = Image.open(f).convert("1")
    
    bid = BinaryImageData(metadata,img)
   
    return bid
    
#####################
# UTILITY
#####################
def string_numbers_to_tuple(string,is_int=False):
    """Takes a number of comma-separated strings and converts it 
        to float by default, or to int if flag is True."""
        
    pos = string.split(",")
    try:
        if is_int:
            return tuple([int(p) for p in pos])
        else:
            return tuple([float(p) for p in pos])
    except:
        return None

def get_key_from_file(filename):
    for file in filename:
        f = open(file,'r')
        key = f.read().strip()
        f.close()
        return key
    
        
#####################
#Main
#####################
if __name__ == "__main__":
    args = sys.argv[1:len(sys.argv)]
    
    parser = argparse.ArgumentParser()
    parser.add_argument("bing_key",nargs=1,help="Name/path of a text file containing only a Bing Maps API key.")
    parser.add_argument("location",nargs='?',default="59.3256954,18.0718675",
        help="Map position, in longitude/latitude degrees. Should be two or four comma-separated floats. "+ \
                "If two, then it is considered center point of the map, in Latitude,Longitude. " + \
                "If four, then it is considered a bounding box. "+\
                "Bounding box degrees are given in SWNE (south,west...) order. Default is area over Stadsholmen.")
    parser.add_argument("size",nargs='?',default="300,300",
        help="Size of map, in pixels in format width,height. Both must be integers. Default is 500,500.")
    parser.add_argument("zoom",nargs='?',type=int,default=13,
        help="Zoom level, must be an integer. 0 is view of full planet, 20 is very zoomed in. Default is 13.")
    parser.add_argument("-s","--save",action='store_true',
        help="if we want to save the bnary image")
        
    args = parser.parse_args()
    
    #import pdb
    #pdb.set_trace()
    
    #convert to numbers
    location = string_numbers_to_tuple(args.location)
    size = string_numbers_to_tuple(args.size,is_int=True)
    zoom = args.zoom
    key = get_key_from_file(args.bing_key)
    
    binary_image_data = construct_binary_image(location,size,zoom,key)
    
    if args.save:
        binary_image_data.binary_img.save("binary_img.png")

    print binary_image_data.bounding_box
    print binary_image_data.center
    print binary_image_data.size
    print binary_image_data.zoom    
    print binary_image_data.binary_img
    print "success!"