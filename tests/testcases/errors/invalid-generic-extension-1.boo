"""
invalid-generic-extension-1.boo(9,8): BCE0146: The type 'int' must be a reference type in order to substitute the generic parameter 'T' in 'Invalid-generic-extension-1Module.Test[of T](T)'.
"""

[Extension]
def Test[of T(class)](source as T):
	print "You should not be here. -- Levelord"
	
42.Test()
