"""
BCE0070-1.boo(6,18): BCE0070: Definition of 'BCE0070_1Module.fatorial(int)' depends on 'BCE0070_1Module.fatorial(int)' whose type could not be resolved because of a cycle. Explicitly declare the type of either one to break the cycle.
"""
def fatorial(value as int):
	return 1 if value < 2
	return value*fatorial(value-1)
	
