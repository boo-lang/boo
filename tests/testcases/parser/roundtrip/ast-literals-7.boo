"""
class Foo:

	node1 = [|
		return 3
	|]


	node2 = [|
		return 42
	|]


print 'it works'
"""
class Foo:
	node1 = [|
		return 3
	|]

	node2 = [| return 42 |]
	
print 'it works'
