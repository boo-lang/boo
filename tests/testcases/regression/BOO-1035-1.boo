"""
OK
OKx
y
y
OKdx
dy
dy
"""

if "":
	print "EMPTY FAIL!"
if not "":
	print "OK"

x = ""
print x if x
print "OKx" if not x
y = "y"
print x or y
print y or x

dx as duck = ""
print dx if dx
print "OKdx" if not dx
dy as duck = "dy"
print dx or dy
print dy or dx

