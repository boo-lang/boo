"""
BCE0019-1.boo(26,3): BCE0019: 'FirstName' is not a member of 'Expando'.
BCE0019-1.boo(27,3): BCE0019: 'LastName' is not a member of 'Expando'.
BCE0019-1.boo(28,3): BCE0019: 'GetName' is not a member of 'Expando'.
BCE0019-1.boo(28,27): BCE0019: 'FirstName' is not a member of 'Expando'.
BCE0019-1.boo(28,42): BCE0019: 'LastName' is not a member of 'Expando'.
BCE0019-1.boo(30,9): BCE0019: 'FirstName' is not a member of 'Expando'.
BCE0019-1.boo(31,9): BCE0019: 'LastName' is not a member of 'Expando'.
BCE0019-1.boo(32,9): BCE0019: 'GetName' is not a member of 'Expando'.
"""
class Expando(IQuackFu):

	_attributes = {}
	
	def QuackInvoke(name as string, args as (object)) as object:
		return (_attributes[name] as callable).Call(args)
		
	def QuackSet(name as string, value) as object:
		_attributes[name] = value
		return value
		
	def QuackGet(name as string) as object:
		return _attributes[name]
		
e = Expando()
e.FirstName = "Homer"
e.LastName = "Simpson"
e.GetName = { return "${e.FirstName} ${e.LastName}" }

print e.FirstName
print e.LastName
print e.GetName()
