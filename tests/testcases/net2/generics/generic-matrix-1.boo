"""
System.String[,]
1
2
"""
m = matrix[of string](1, 2)
print m.GetType()
for i in range(m.Rank):
	print m.GetLength(i)
