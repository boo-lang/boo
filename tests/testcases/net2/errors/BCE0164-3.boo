"""
BCE0164-3.boo(14,7): BCE0164: Cannot infer generic arguments for method 'BCE0164_3Module.Method[of T](T, T)'. Provide stronger type information through arguments, or explicitly state the generic arguments.
"""

class Class1:
	pass
	
class Class2:
	pass
	
def Method[of T](arg1 as T, arg2 as T):
	pass
	
Method(Class1(), Class2())
