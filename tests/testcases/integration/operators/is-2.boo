"""
OK
OK2
"""

def True():
	return true

x = true
if x is true and True() is true:
	print "OK"
if x is false and True() is true:
	print "KO"
if x is not true or True() is not true:
	print "KO2"
if x is not false and True() is not false:
	print "OK2"

