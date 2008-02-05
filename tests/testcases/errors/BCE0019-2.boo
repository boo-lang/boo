"""
BCE0019-2.boo(16,3): BCE0019: 'MakeSometing' is not a member of 'Test'. Did you mean 'MakeSomething' ?
BCE0019-2.boo(17,3): BCE0019: 'MakNothing' is not a member of 'Test'. 
BCE0019-2.boo(18,9): BCE0019: 'MakNothing' is not a member of 'Test'. Did you mean 'MakeNothing' ?
"""

class Test:
	def MakeSomething():
		pass

	MakeNothing:
		get:
			return false

t = Test()
t.MakeSometing()
t.MakNothing()
print t.MakNothing
