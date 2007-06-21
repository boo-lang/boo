"""
QuackSet(FirstName)
QuackSet(LastName)
QuackSet(GetName)
QuackGet(FirstName)
Homer
QuackGet(LastName)
Simpson
QuackInvoke(GetName)
QuackGet(FirstName)
QuackGet(LastName)
Homer Simpson
op_Subtraction GetName
"""
class Expando(IQuackFu):

	_attributes = {}
	
	def Remove(attrName as string):
		_attributes.Remove(attrName)
	
	def QuackInvoke(name as string, args as (object)) as object:
		print "QuackInvoke(${name})"
		return (_attributes[name] as callable).Call(args)
		
	def QuackSet(name as string, parameters as (object), value) as object:
		assert parameters is null
		print "QuackSet(${name})"
		_attributes[name] = value
		return value
		
	def QuackGet(name as string, parameters as (object)) as object:
		assert parameters is null
		print "QuackGet(${name})"
		return _attributes[name]
		
	static def op_Subtraction(lhs as Expando, rhs as string):
		print "op_Subtraction ${rhs}"
		lhs.Remove(rhs)
		return lhs
		
e = Expando()
e.FirstName = "Homer"
e.LastName = "Simpson"
e.GetName = { return "${e.FirstName} ${e.LastName}" }

print e.FirstName
print e.LastName
print e.GetName()

e -= "GetName"
