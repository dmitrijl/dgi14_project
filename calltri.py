
from laspy import file

if __name__ == "__main__":
    lasf = file.File("a.las", mode = "r")
    v_count = len(lasf)
    x = lasf.get_x_scaled()
    y = lasf.get_y_scaled()
    z = lasf.get_z_scaled()
    
    v_count = 10 #remove this line later
    pfile = open("a.poly", "w")
    pfile.write("%i 2 1 0\n" % v_count)
    for i in xrange(v_count):
        pfile.write("%i %s %s %s\n" % (i+1,x[i],y[i],z[i]))
        
    pfile.write("0 0\n")
    pfile.write("0\n")
    pfile.close()
    lasf.close()
    
    
     
    
    