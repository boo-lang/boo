"""
120

"""
def fatorial(n as int):
	return 1 if n < 2
	return n * fatorial(n-1)
	
System.Console.WriteLine(fatorial(5))
