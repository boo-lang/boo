"""
1, bar
2, baz
3, foo
null
null
1
2
3
4
5
8
null
1, 1
1, 2
1, 2, 3
1, 2, 4
1, 3
"""
def sort(items as List):
	for item in items.Sort():
		if item is null:
			print("null")
			continue
			
		if item isa System.Array:
			print(join(item, ", "))
			continue
		
		print(item)

sort([(3, "foo"), (1, "bar"), (2, "baz")])
sort([4, 5, 8, null, 3, 2, null, 1])
sort([(1, 2, 3), null, (1, 2, 4), (1, 3), (1, 1), (1, 2)])
	
