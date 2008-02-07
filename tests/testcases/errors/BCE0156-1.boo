"""
BCE0156-1.boo(19,10): BCE0156: Event 'E' can only be invoked from within the class it was declared ('Base').
BCE0156-1.boo(22,11): BCE0156: Event 'SE' can only be invoked from within the class it was declared ('Base').
"""

class Base:
	public event E as System.EventHandler
	public static event SE as System.EventHandler

	def GoodRaiseE():
		E(null, null)

	static def GoodRaiseSE():
		SE(null, null)


class Derived(Base):
	def WrongRaiseE():
		E(null, null)

	static def WrongRaiseSE():
		SE(null, null)
