"""
BCW0007-1.boo(6,5): BCW0007: WARNING: Assignment inside a conditional. Did you mean '==' instead of '=' here: 'a = b'?
"""
a = false
b = true
if a=b:
	print "ok"

x = System.Console.ReadLine()
o = (1 if len(x) > 2 else m = 3) #No warning here
print o, m


