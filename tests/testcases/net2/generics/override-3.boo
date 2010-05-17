class Base:
	virtual def Method[of T](arg as T):
		return arg

class Derived(Base):
	override def Method[of U](arg as U):
		return arg

assert typeof(Derived).GetMethod("Method").GetBaseDefinition() == typeof(Base).GetMethod("Method")