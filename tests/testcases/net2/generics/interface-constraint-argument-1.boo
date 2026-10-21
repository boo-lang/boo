"""
Reader
"""
interface IThing:
	pass

class Reader[of T(IThing)]:
	override def ToString():
		return "Reader"

class Source:
	def Open[of T(IThing)]() as Reader[of T]:
		return Reader[of T]()

class Thing(IThing):
	pass

print Source().Open[of Thing]()
