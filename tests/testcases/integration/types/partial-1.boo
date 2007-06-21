"""
method1
field1
property1
property2
inheritedmethod
method1
"""

interface ITest:
	def method1()
	
class BaseClass:
	def inheritedmethod():
		print "inheritedmethod"

partial class C:
	def method1():
		print "method1"
	
	Property1:
		get:
			return "property1"

//this can be in a separate file
partial class C(BaseClass, ITest):
	public fld1 = "field1"
	
	def method2():
		print "method2"
		
	Property2:
		get:
			return "property2"

c = C()
c.method1()
print c.fld1
print c.Property1
print c.Property2
c.inheritedmethod()
test = c as ITest
test.method1()

