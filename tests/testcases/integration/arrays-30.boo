"""
System.Int32[]
System.Int32[,]
"""
a1 as (int, 1)
a1 = array(int, 1)
print(a1.GetType())

a2 as (int, 2)
a2 = System.Array.CreateInstance(int, 2, 2)
print(a2.GetType())

