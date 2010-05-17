"""
bar
"""
class Foo:
	
	[getter(Bar)]
	_a = Clickable(Name: "bar", Clicked: foo)
	
	def foo():
		print _a.Name
		
class Clickable:
	
	[property(Name)]
	_name as string
	
	event Clicked as callable()
	
	def Click():
		Clicked()

Foo().Bar.Click()
		
