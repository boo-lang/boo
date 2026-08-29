"""
True
"""
class C2(C1):
	pass
	
class C1:
	pass
	
System.Console.Write(typeof(C2).BaseType is typeof(C1))
