"""
t2 of type System.Int32[,]
1
0
2
0
3
0
t3 of type System.Int32[,,]
1
0
2
0
3
0
"""

t1 = matrix of int(4, 4, 4)

t1[0,0,0]=1
t1[0,1,0]=2
t1[0,2,0]=3

t2 = t1[0,0:3,0:2]
t3 = t1[0:1,0:3,0:2]

print "t2 of type ${t2.GetType()}"
for i in t2:
	print i

print "t3 of type ${t3.GetType()}"
for i in t3:
	print i
