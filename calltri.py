
from laspy import file
from math import *
import os
import subprocess
import sys
import numpy


#Arguments: x, y, z and classification arrays of a set of nodes
def downsample(nlx, nly, nlz, nlc, vg_cellcount_x = 250, vg_cellcount_y = 250):
    #Find the minimum and maximum coordinates
    xmax = ymax = -float("inf")
    xmin = ymin = float("inf")
    nr_nodes = len(nlx)
    #for i in xrange(nr_nodes):
    xmax = max(nlx)
    print "xmax done"
    ymax = max(nly)
    print "ymax done"
    xmin = min(nlx)
    print "xmin done"
    ymin = min(nly)
    print "ymax done"
    xdiff = xmax-xmin
    ydiff = ymax-ymin
    
    #Create a voxelgrid
    #Number of cells, and their size, in each dimension:
    #vg_cellcount_x = 250
    #vg_cellcount_y = 250    #250*250 = 62500 < 65000
    vg_cellsize_x = (xmax-xmin)/vg_cellcount_x
    vg_cellsize_y = (ymax-ymin)/vg_cellcount_y
    vgx = alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgy = alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgz = alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgc = alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgb = alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    
    
    vgx_avg = [] #alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgy_avg = [] #alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgz_avg = [] #alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    vgc_avg = [] #alloc_matrix(vg_cellcount_y, vg_cellcount_x)
    
    averages = []
    print "allocated"
    
    #Put the nodes in their corresponding cell
    for i in xrange(nr_nodes):
        xc = vg_cellcount_x*(nlx[i]-xmin)/xdiff
        xc = trunc(xc)
        xc = max(xc, 0) #Prevent out of bounds
        xc = min(xc, (vg_cellcount_x-1))    #Prevent out of bounds
        yc = vg_cellcount_y*(nly[i]-ymin)/ydiff
        yc = trunc(yc)
        yc = max(yc, 0) #Prevent out of bounds
        yc = min(yc, (vg_cellcount_y-1))    #Prevent out of bounds
        
        vgx[yc][xc] += nlx[i]
        vgy[yc][xc] += nly[i]
        vgz[yc][xc] += nlz[i]
        vgc[yc][xc] += nlc[i]        
        vgb[yc][xc] += 1        
        
        
        #vgx[yc][xc].append(nlx[i])
        #vgy[yc][xc].append(nly[i])
        #vgz[yc][xc].append(nlz[i])
        #vgc[yc][xc].append(nlc[i])
        if i%100000 == 0:
            print "placed %i nodes in bins"%i
    
    #Compute average in each cell
    for i in xrange(vg_cellcount_y):
        for j in xrange(vg_cellcount_x):
            node_count = vgb[i][j]
            if node_count != 0:
                vgx_avg.append(vgx[i][j]/node_count)
                vgy_avg.append(vgy[i][j]/node_count)
                vgz_avg.append(vgz[i][j]/node_count)
                
                #vgx_avg[i][j] = sum(vgx[i][j])/node_count
                #vgy_avg[i][j] = sum(vgy[i][j])/node_count
                #vgz_avg[i][j] = sum(vgz[i][j])/node_count
            #TODO special case for vgc
            
    return (vgx_avg, vgy_avg, vgz_avg, vgc_avg)
    #TODO not tested yet
    

def alloc_matrix_list(W, H):
    return [ [ [] for i in range(W) ] for j in range(H) ]
    
def alloc_matrix_2list(W, H):
    return [ [ [0,0.0] for i in range(W) ] for j in range(H) ]

def alloc_matrix(W, H):
    return [ [ 0 for i in range(W) ] for j in range(H) ]
    
def las_to_xyzc(filename, v_c = -1):
    #lasf = file.File(filename, mode = "r")
    #_files_open.append(lasf)
    v_count = len(lasf)
    x = lasf.get_x_scaled()
    y = lasf.get_y_scaled()
    z = lasf.get_z_scaled()
    c = lasf.get_raw_classification()
    if v_c > 0:
        x = x[0:v_c]
        y = y[0:v_c]
        z = z[0:v_c]
        c = c[0:v_c]
    #lasf.close()
    return (x,y,z,c)
    
def write_xyzc_to_poly(x,y,z,c, name = "res.poly"):
    #TODO do something with c
    v_count = len(x)
    pfile = open(name, "w")
    pfile.write("%i 2 1 0\n" % v_count)
    for i in xrange(v_count):
        pfile.write("%i %s %s %s\n" % (i+1,x[i],y[i],z[i]))
        
    pfile.write("0 0\n")
    pfile.write("0\n")
    pfile.close()
    

def create_poly_file(lasf, v_c = -1):
    lasf = file.File(lasfilename, mode = "r")
    v_count = len(lasf)
    x = lasf.get_x_scaled()
    y = lasf.get_y_scaled()
    z = lasf.get_z_scaled()
    
    #v_count = 10000 #remove this line later
    if v_c > 0:
        v_count = v_c
    pfile = open("%s.poly"%os.path.splitext(lasfilename)[0], "w")
    pfile.write("%i 2 1 0\n" % v_count)
    for i in xrange(v_count):
        pfile.write("%i %s %s %s\n" % (i+1,x[i],y[i],z[i]))
        
    pfile.write("0 0\n")
    pfile.write("0\n")
    pfile.close()
    lasf.close() 

def combined_polys(filenames):
    readers = []
    for f in filenames:
        readers.append(file.File(f,mode = "r"))
        
    total_v = 0
    for r in readers:
        total_v += len(r)
        
    pfile = open("full.poly", "w")
    pfile.write("%i 2 1 0\n"%total_v)
    i = 1
    for k,r in enumerate(readers):
        x = lasf.get_x_scaled()
        y = lasf.get_y_scaled()
        z = lasf.get_z_scaled()
        c = len(r)
        for j in xrange(c):
            pfile.write("%i %s %s %s\n" % (i,x[j],y[j],z[j]))
            i += 1
        print "file nr %i done"%k
        
    pfile.write("0 0\n")
    pfile.write("0\n")
    
    for r in readers:
        r.close()
    pfile.close()

def triangulate(f,q = False):
    if q:
        s = subprocess.check_output(["tri.exe","-pqc",f])
        #s = subprocess.check_output(["triangle","-pqc",f])
    else:
        s = subprocess.check_output(["tri.exe","-pc",f])
        #s = subprocess.check_output(["triangle","-pc",f])
    print s
    
if __name__ == "__main__":
    if len(sys.argv) >= 2:
        i = int(sys.argv[1])
    else:
        i = -1
    
    q = False
    if len(sys.argv) >= 3:
        if sys.argv[2] == "q":
            q = True
    
    las_files = [
        "a.las",
        #"b.las",
        #"c.las",
        #"d.las",
    ]
    
    x = []
    y = []
    z = []
    c = []
    
    print "reading las files"
    open_files = []
    for f in las_files:
        lasf = file.File(f, mode = "r")
        open_files.append(lasf)
        x,y,z,c = las_to_xyzc(lasf,i)
        """x_t, y_t, z_t, c_t = las_to_xyzc(f,i)
        x.extend(x_t)
        y.extend(y_t)
        z.extend(z_t)
        c.extend(c_t)
        """
    print "Downsampling"
    xd,yd,zd,cd = downsample(x,y,z,c)
    print "Downsampling done"
    
    for f in open_files:
        f.close()
    
    print "Creating res.poly"
    
    write_xyzc_to_poly(xd,yd,zd,cd)
    
    
    #create_poly_file("a.las",i)
    #create_poly_file("b.las")
    #create_poly_file("c.las")
    #create_poly_file("d.las")
    print "poly file created, triangulate now"
    triangulate("res",q)
    
    
     
    
    
