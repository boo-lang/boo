class Test2Base:
	virtual def Do() as string:
		pass
	virtual def Do[of T]() as T:
		pass

class Test2(Test2Base):
	def Do() as string:
		return "non-generic"
	def Do[of T]() as T:
		pass

t = Test2()
assert "non-generic" == t.Do()
#t.Do[of int]()

