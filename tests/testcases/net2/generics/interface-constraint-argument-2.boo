"""
Reader
"""
interface IThing:
	pass

interface IDerived(IThing):
	pass

class Reader[of T(IThing)]:
	override def ToString():
		return "Reader"

class Source:
	def Describe[of T(IDerived), U(T)](reader as Reader[of T], other as Reader[of U]):
		print reader

class Thing(IDerived):
	pass

Source().Describe[of Thing, Thing](Reader[of Thing](), Reader[of Thing]())
