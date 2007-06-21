"""
node1 as Node = [|
	return 3
|]

node2 as Node = [|
	return 42
|]

print 'it works'
"""
node1 as Node = [|
	return 3
|]

node2 as Node = [| return 42 |]

print 'it works'
