"""
55
"""

class Class:
	def Method() as int:
		def Recursive(n as int) as int:
			return 0 unless n
			return n + Recursive(n - 1)
			
		return Recursive(10)

print Class().Method()
