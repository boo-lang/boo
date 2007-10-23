class Foo(ICharacter):
	[AutoDelegate(ICharacter)]
	Character as Character

[method_decorator(Character.dump, Customer)]
class Trace:
	include TraceMixin

mixin TraceMixin(Foo, ICollection):
	
	def before(name):
		print(name)

	def after(name):
		print(name)
		
mixin ListMixin(IList):

	include CollectionMixin
	
		
[class_invariant(Count > -1)]
[demands_attribute(Foo)]
class Base:
	Count as int
	
for line in TextFile("foo"):	
	given line:
		when /$foo/:
			print("cool!")
			
		when /#def/:
			print("hello, master.")
			
		when cool_enough:
			print("cool!")
			
		when _prop1:
			print("prop1")
		
		
def cool_enough(s as string):
	pass
		
	
