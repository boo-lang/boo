class A:
	virtual def @then():
		return "A.then"

	def @get(prop as string):
		return prop
	def @set(prop as string, value as int):
		return @get(prop) + ':' + value
		
class @class(A):
	override def @then():
		return "B:" + super()

enum @try:
	foo
	bar

def @while(msg):
	return msg
		
a = A()
b = @class()
t = @try.bar

assert "A.then" == a.@then()
assert "prop" == a.get('prop')
assert "prop" == a.@get('prop')
assert "prop:10" == a.set('prop', 10)
assert "B:A.then" == b.@then()
assert "imp" == @while('imp')
assert @try.bar == t
