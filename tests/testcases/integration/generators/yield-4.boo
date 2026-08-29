"""
2
1
0
"""
def onetwothree():
	i = 3
	yield --i
	yield --i
	yield --i
	
e = onetwothree().GetEnumerator()
while e.MoveNext():
	print(e.Current)
