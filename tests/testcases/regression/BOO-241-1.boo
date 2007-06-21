class Base():
	public pubs as string

class Child(Base):

	class ChildBase():
		pass
		
	class ChildChild(Child.ChildBase):
		pass

	public stuff as string

	def foo():
		return "foo"

class StepChild(Base):

	public stuff as string

	def foo():
		return "foo"

	class ChildBase():
		pass
	
class ChildChild(StepChild.ChildBase):
	pass
	
def AssertBaseType(type as System.Type, expected as System.Type):
	assert type.BaseType is expected

AssertBaseType(Base, object)
AssertBaseType(Child, Base)
AssertBaseType(Child.ChildBase, object)
AssertBaseType(Child.ChildChild, Child.ChildBase)
AssertBaseType(StepChild, Base)
AssertBaseType(StepChild.ChildBase, object)
AssertBaseType(ChildChild, StepChild.ChildBase)
