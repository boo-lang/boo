"""
System.Object[]
System.Int32[]
"""
t1 = tuple(range(3))
t2 = tuple(range(3), int) as (int)

print(t1.GetType())
print(t2.GetType())
