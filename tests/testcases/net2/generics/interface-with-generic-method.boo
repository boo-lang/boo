"""
Bar
"""
interface IInterface:
	def TemplateFoo[of T]()

class Class1(IInterface):
	def TemplateFoo[of T]():
		print typeof(T).Name
		
class Bar:
	pass
	
def doIt(i as IInterface):
	i.TemplateFoo of Bar()
	
doIt(Class1())
