import sys      #for main()
import argparse #for parsing args

import urllib2
import Image
import json

class ImageInfo(object):
    
    @property
    def center(self):
        """(x,y) - two doubles"""
        return _center
        
    @property
    def size(self):
        """(width,height) - two integers"""
        return _size
        
    @property
    def zoom(self):
        """zoom - an integer"""
        return _zoom
        
    @property
    def is_center(self):
        """If this location is given by center."""
        return len(self.center) == 2
        
    def __init__(center,size,zoom):
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
        return self._ii
        
    @property
    def center(self):
        return self._ii.center
        
    @property
    def size(self):
        return self._ii.size
        
    @property
    def zoom(self):
        return self._ii.zoom
        
    def __init__(self,bounding_box,image_info):
        """Bounding_box should be a tuple of len 4, image_info is an ImageInfo object."""
        self._bb = bounding_box
        self._ii = image_info
        
    
    
class BinaryImageInfo(object):
    
    @property
    def metadata(self):
        """Returns the original BingImageMetaData this object was constructed from."""
        return self._ii
        
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
    ii = ImageInfo(location,size,zoom)
    metadata = get_bing_image_meta_data(ii)
    binary_image_data = get_binary_image_info(metadata,bing_key)
    return binary_image_data

def get_bing_image_meta_data(image_info,bing_key):
    """Calls Bing static map API and gets some metadata info about a the given ImageInfo.
    
    The metadata is returned as a BingImageMetaData object."""
    #example:
    #https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/59.3256954,18.071867/13?mapMetadata=1&o=xml&key=bing_key
    if image_info.is_center:
        url = "https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/"
        center = str(image_info.center[0]) + "," + str(image_info.center[1]) + "/"
        url += center
        url += str(image_info.zoom)
        size = str(image_info.size[0]) + "," + str(image_info.size[1])
        url += "?mapSize=" + size + "&mapMetadata=1&key=" + bing_key
        f = urllib2.urlopen(url).read()
        obj = json.loads(f)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
    
    pass #TODO
    
def get_binary_image_info(metadata):
    """Calls Google Static Map API with the given image metadata, returns a BinaryImageInfo."""
    pass #TODO
    
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
    f = open(filename,'r')
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
        
    args = parser.parse_args()
    
    #convert to numbers
    location = string_numbers_to_tuple(args.location)
    size = string_numbers_to_tuple(args.size)
    zoom = args.zoom
    key = get_key_from_file(args.bing_key)
    
    binary_image_data = construct_binary_image(location,size,zoom,key)

    
    print "success!"