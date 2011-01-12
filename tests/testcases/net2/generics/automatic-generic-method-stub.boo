"""
nudge, nudge
"""
interface IFoo:
	def Bar[of T](ts as (T)) as T*
	
class Foo(IFoo):
	pass
	
f = Foo()
try:
	f.Bar[of string]((,))
except x as System.NotImplementedException:
	print "nudge, nudge"
	
