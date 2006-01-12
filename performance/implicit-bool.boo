class Foo:
	static def op_Implicit(v as Foo) as bool:
		return not v.Value
		
	[property(Value)]
	_value as bool
	
class Bar:
	pass

def time(description as string, block as callable()):
	start = date.Now
	for i in range(10000000):
		block()
	end = date.Now
	print description, "took ${end-start}"
	
b = Bar()
time("no conversion defined") do:
	while b:
		break

f = Foo()
time("typed implicit conversion") do:
	while f:
		break
		
d1 as duck = b
time("duck no conversion defined") do:
	while d1:
		break

d2 as duck = f
time("duck implicit conversion") do:
	while d2:
		break

i as duck = 1
time("int-to-bool conversion") do:
	while i:
		break
