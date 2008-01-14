"""
FORLOOP1 GOOD
FORLOOP2 GOOD
METHOD GOOD
TRY GOOD
OK
"""
class Test:

	def ForLoopsWithUnreachableCode():
		for i in range(0, 1):
			print "FORLOOP1 GOOD"
			continue
			print "FORLOOP1 BAD"
		for i in range(0, 1):
			print "FORLOOP2 GOOD"
			break
			print "FORLOOP2 BAD"

	def MethodWithUnreachableCode():
		print "METHOD GOOD"
		return
		print "METHOD BAD"

	def TryBlockWithUnreachableCode():
		try:
			print "TRY GOOD"
			raise System.Exception()
			print "TRY BAD"
		except:
			print "OK"

t = Test()
t.ForLoopsWithUnreachableCode()
t.MethodWithUnreachableCode()
t.TryBlockWithUnreachableCode()
