"""
Internal
"""

class Base[of T(Base[of T])]:
	def Run():
		print typeof(T).Name

class Internal(Base[of Internal]):
	pass

Internal().Run()

