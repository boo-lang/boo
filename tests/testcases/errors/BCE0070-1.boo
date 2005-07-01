"""
BCE0070-1.boo(6,18): BCE0070: Definition of 'BCE0070-1Module.fatorial' depends on 'BCE0070-1Module.fatorial' whose type could not be resolved because of a cycle. Explicitly declare the type of either one to break the cycle.
"""
def fatorial(value as int):
	return 1 if value < 2
	return value*fatorial(value-1)
	
