enum E:
	a
	b
	c

def check(val as System.Enum):
	assert val != null
	assert E.a.Equals(val)

check(E.a)
