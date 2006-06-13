"""
10
10
11
11
12
12
in static f
12
12
9
8
7
7 7 7
in f
in static f
"""
class X:
	
	public i = 0
	
	static public istatic = 0
	
	[property(p)]
	_p
	
	[property(ps)]
	static _ps
	
	def f():
		return "in f"
		
	static def fstatic():
		return "in static f"
		
class Y(X):
	pass
	
o = X(i: 10)
o.p = 11
o.istatic = 12

print o.i
print ((o as duck).i)
print o.p
print ((o as duck).p)
print ((o as duck).istatic)
print ((X as duck).istatic)
print ((X as duck).fstatic())
print Y.istatic
print ((Y as duck).istatic)
print (Y.istatic = 9)
print ((Y as duck).istatic = 8)
print ((Y as duck).ps = 7)
print o.ps, Y.ps, (Y as duck).ps
print ((Y() as duck).f())
print ((Y as duck).fstatic())
