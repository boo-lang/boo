"""
Homer
Simpson
Homer Simpson
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
