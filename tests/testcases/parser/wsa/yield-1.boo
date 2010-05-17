"""
def odds(l):
	for i in l:
		yield i if (0 != (i % 2))

def d(i):
	return (i * 2)

def map(fn, enumerable):
	for item in enumerable:
		yield fn(item)

for odd in map(d, odds([1, 2, 3, 4, 5])):
	print(odd)
"""
def odds(l):
	for i in l:
		yield i if (0 != (i % 2))
	end
end

def d(i):
	return (i * 2)
end

def map(fn, enumerable):
	for item in enumerable:
		yield fn(item)
	end
end

for odd in map(d, odds([1, 2, 3, 4, 5])):
	print(odd)
end
