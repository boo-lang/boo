"""
0
 1 3 5 7 9
2
 3 5 7 9
4
 5 7 9
6
 7 9
8
 9
"""
import System.Console

for i in range(10):
	continue if i % 2
	WriteLine(i)
	for j in range(i, 10):
		continue if 0 == j % 2
		Write(" ${j}")
	WriteLine()
