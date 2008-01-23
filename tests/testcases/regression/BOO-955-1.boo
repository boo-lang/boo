"""
ok
"""
interface ITest:
	def Method(s as string) as string:
		pass

class Test(ITest):
	def Method() as string:
		pass

print "ok"
