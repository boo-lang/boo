import System

functions = Math.Sin, Math.Cos

a = []
for f in functions:
	a.Add(f(i) for i in range(3))
	
for iterator in a:
	print(join(iterator))
