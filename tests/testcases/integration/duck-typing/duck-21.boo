"""
True True
"""
class Test:

	[property(enabled)]
	_enabled = false
	
a as duck = Test()
b as duck = Test()

a.enabled = b.enabled = true
print a.enabled, b.enabled
