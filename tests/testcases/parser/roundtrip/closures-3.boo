"""
def adder(amount as int):
	return { value as int | return (value + amount) }

a = adder(3)
"""
def adder(amount as int):
	return def (value as int):
		return value+amount

a = adder(3)
