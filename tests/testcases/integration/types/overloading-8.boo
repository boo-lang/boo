class Test[of T]:
	pass
class Test[of T,U]:
	pass
class Test[of T,U,V]:
	pass

Test[of int]()
Test[of int,int]()
Test[of int,string,string]()
