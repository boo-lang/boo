"""
System.Object[]
System.Int32[]
"""
t1 = tuple(range(3))
t2 = tuple(int, range(3)) as (int)

print(t1.GetType())
print(t2.GetType())
