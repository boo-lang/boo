"""
BCE0120-5.boo(24,9): BCE0120: 'PrivateSetter.set_PrivateSet' is inaccessible due to its protection level.
BCE0120-5.boo(29,19): BCE0120: 'ProtectedSetter.set_ProtectedSet' is inaccessible due to its protection level.
BCE0120-5.boo(32,17): BCE0120: 'ProtectedSetter.set_ProtectedSet' is inaccessible due to its protection level.
BCE0120-5.boo(34,17): BCE0120: 'PrivateSetter.set_PrivateSet' is inaccessible due to its protection level.
"""
class ProtectedSetter:
	ProtectedSet:
		get:
			return 1
		protected set:
			pass

class PrivateSetter (ProtectedSetter):
	PrivateSet:
		get:
			return 1
		private set:
			pass

class UsingSetter (PrivateSetter):
	def Foo():
		ProtectedSet = 1
		PrivateSet = 1 #ERR


#internal type
print ProtectedSetter().ProtectedSet
ProtectedSetter().ProtectedSet = 1 #ERR

print PrivateSetter().ProtectedSet
PrivateSetter().ProtectedSet = 1 #ERR
print PrivateSetter().PrivateSet
PrivateSetter().PrivateSet = 1 #ERR

