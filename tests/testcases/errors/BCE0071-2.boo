"""
BCE0071-2.boo(10,14): BCE0071: Inheritance cycle detected: 'IA'.
"""
interface IA(IB):
	pass
	
interface IB(IC):
	pass
	
interface IC(IA):
	pass
	

