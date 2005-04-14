class XPer:

	def Test():
		while true:
			print "testing"
			yield null
			
	def Code():
		while true:
			print "coding"
			yield null
			
	def Refactor():
		while true:
			print "refactoring"
			yield null
			
	def Iterate(times as int):
		assert times > 0
		
		test = Test().GetEnumerator()
		code = Code().GetEnumerator()
		refactor = Refactor().GetEnumerator()
		while times > 0:
			test.MoveNext()
			code.MoveNext()
			refactor.MoveNext()
			test.MoveNext()
			--times
			
			
XPer().Iterate(5)
