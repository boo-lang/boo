"""
System.Int32[,]
System.String[,]
System.Object[,]
"""

t1 = matrix(int, 2, 3)
t2 = matrix(string, 3, 3)
t3 = matrix(object, 3, 3)

print(t1.GetType())
print(t2.GetType())
print(t3.GetType())
