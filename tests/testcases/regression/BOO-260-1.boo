"""
42
True
14
0
"""
// expecting 'as string' return type
callable I2S(i as int) as string
callable I2I(i as int) as int

def i2s(cb as I2S):
	print cb(42) is null
	
def i2i(cb as I2I):
	print cb(14)

def func1(i as int) as void:
	print i

i2s(func1)
i2i(func1)
