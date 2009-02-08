#ignore FIXME: Covariance support is incomplete, generates unverifiable IL (BOO-1155)
"""
OK
"""
#BOO-1155
class Base:
	pass

class Derived (Base):
	pass

class Foo[of T]:
	pass

def Method(x as Foo[of Base]):
	print "OK"

f = Foo[of Derived]()
Method(f)

