"""
BCE0000-1.boo(10,7): BCE0000: Cannot introduce AbstractCollection base class.
"""
import Useful.Collections from Boo.Lang.Useful

class C:
	pass
	
[collection(string)]
class StringCollection(C):
	pass

s = StringCollection()

