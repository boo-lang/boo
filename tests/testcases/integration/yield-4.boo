"""
1
2
3
"""
def onetwothree():
	i = 0
	yield ++i
	yield ++i
	yield ++i
	
e = onetwothree().GetEnumerator()
while e.MoveNext():
	print(e.Current)
