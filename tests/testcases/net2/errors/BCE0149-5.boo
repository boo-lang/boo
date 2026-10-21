"""
BCE0149-5.boo(11,25): BCE0149: The type 'T' must derive from 'IThing' in order to substitute the generic parameter 'T' in 'Reader[of T]'.
"""
interface IThing:
	pass

class Reader[of T(IThing)]:
	pass

class Source:
	def Open[of T]() as Reader[of T]:
		return null
