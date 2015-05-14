
from laspy import file
import os
import subprocess
import sys

def create_poly_file(lasfilename, v_c = -1):
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
    
    
    create_poly_file("a.las",i)
    #create_poly_file("b.las")
    #create_poly_file("c.las")
    #create_poly_file("d.las")
    print "poly file created, triangulate now"
    triangulate("a",q)
    
    
     
    
    
