[ConManager]
class Character:
	[ConVolatile]
	Name as string
	
	Name as string:
		get:
			Name
			
		set:
			Name = value
			
	def dump():
		print("${Name}")
		
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

def inorder(tree):
	return unless tree
	yield node for node in inorder(tree.left)
	yield tree
	yield node for node in inorder(tree.right)

def inorder(tree):
	if tree:
		for node in inorder(tree.left):
			yield node
		yield tree
		for node in inorder(tree.right):
			yield node
		
def inorder(tree):
	return unless tree

	node = tree.left	
	while node:
		push(node)
		node = node.left
		
	while node=pop():
		yield node
		
	yield tree
		
	node = tree.right
	while node:
		push(node)
		node = node.right
		
	while node=pop()
		yield node
		
def inorder_cat(tree):
	return cat(
		inorder_cat(tree.left),
		(tree,),
		inorder_cat(tree.right)
		) if tree
	
	
	switch (foo())
	{
		case 3:
		{
			return 5;
		}
	}
	
given foo():
		
	when 3:
		return 5
	
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
	
		
	
