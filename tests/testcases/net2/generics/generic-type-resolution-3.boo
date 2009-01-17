"""
Generic`1[System.Int32]
Generic`2[System.Int32,System.String]
Generic`3[System.Int32,System.String,System.DateTime]
"""

class Generic[of T1]: 
	pass
class Generic[of T1, T2]: 
	pass
class Generic[of T1, T2, T3]: 
	pass

g1 = Generic[of int]()
g2 = Generic[of int, string]()
g3 = Generic[of int, string, date]()

print g1.GetType()
print g2.GetType()
print g3.GetType()