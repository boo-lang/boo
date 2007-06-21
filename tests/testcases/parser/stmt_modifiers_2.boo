def fatorial(n as int) as int:
	return 1 if n < 2
	return n*fatorial(n-1)

f = fatorial(5)/fatorial(2)
print(f)
