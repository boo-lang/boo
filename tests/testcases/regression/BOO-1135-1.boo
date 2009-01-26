"""
OK
20
10
"""

[extension] #string has a builtin implicit conversion (BOO-1035)
def op_Implicit(s as string) as bool:
	return s.Length > 1

[extension] #nullables have a builtin implicit conversion too
def op_Implicit(x as int?) as bool:
	return x > 10

x = "x"
y = "OK"
print x or y

a as int? = 10
b as int? = 20
print a or b

c as long? = 10L
d as long? = 20L
print c or d #not using extension since type is 'long?' not 'int?'

