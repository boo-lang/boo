"""
1 1
2 2
3 3
"""
def onetwothree():
	i = 0
	yield ++i
	yield ++i
	yield ++i
	
e1 = onetwothree().GetEnumerator()
e2 = onetwothree().GetEnumerator()
while e1.MoveNext() and e2.MoveNext():
	print("${e1.Current} ${e2.Current}")
