"""
method invocations check for null values (OpCodes.Callvirt)
this is the end
"""
class Foo:
	def Bar():
		return "Foo.Bar"
	
try:
	(null as Foo).Bar()
	print "never gets here"
except x as System.NullReferenceException:
	print "method invocations check for null values (OpCodes.Callvirt)"
	
print "this is the end"
