"""
Homer
Simpson
Homer Simpson
"""
class Expando(IQuackFu):

	_attributes = {}
	
	def QuackInvoke(name as string, args as (object)) as object:
		return (_attributes[name] as callable).Call(args)
		
	def QuackSet(name as string, parameters as (object), value) as object:
		assert parameters is null
		_attributes[name] = value
		return value
		
	def QuackGet(name as string, parameters as (object)) as object:
		assert parameters is null
		return _attributes[name]
		
e = Expando()
e.FirstName = "Homer" # e.QuackSet('FirstName', ('Homer',))
e.LastName = "Simpson" # e.QuackSet
e.GetName = { return "${e.FirstName} ${e.LastName}" }

print e.FirstName # e.QuackGet('FirstName')
print e.LastName
print e.GetName() # e.QuackInvoke('GetName', (,))
