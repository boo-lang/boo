"""
System.Int32[]
System.String[]
System.Object[]
System.Object[]
"""
t1 = (1, 2, 3)
t2 = ("foo", "bar")
t3 = (1, "foo")
t4 = (null, 1, "foo", null)

print(t1.GetType())
print(t2.GetType())
print(t3.GetType())
print(t4.GetType())
