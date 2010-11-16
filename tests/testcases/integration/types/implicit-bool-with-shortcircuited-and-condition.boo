"""
b1.parent
op_Implicit(p1)
p1.on
b1.parent
p1.child
op_Implicit(c1)
c1.on
off
*********
b2.parent
op_Implicit(p2)
p2.on
b2.parent
p2.child
op_Implicit(c2)
c2.on
b2.parent
p2.child
c2.on
parent.child.on
*********
b3.parent
op_Implicit(p3)
p3.on
off
*********
b4.parent
op_Implicit(null)
off
*********
"""
class Togglable:
	static def op_Implicit(o as Togglable) as bool:
		if o is null:
			print "op_Implicit(null)"
			return false
		print "op_Implicit($(o.name))"
		return o.on
		
	public name as string
	traceable_property on as bool
	
class Component(Togglable):
	traceable_property parent as Component
	traceable_property child as Component

macro traceable_property:
	case [| traceable_property $name as $type |]:
		
		backingField = [|
			private $("_$name") as $type
		|]
		yield backingField
		
		yield [|
			public $name:
				get: 
					print "$name." + $(name.ToString())
					return $backingField
				set: $backingField = value
		|]
		
class Behavior(Component):
	def Start():
		if parent and parent.child and parent.child.on:
			print "parent.child.on"
		else:
			print "off"
		print "***" * 3
			
p1 = Component(name: "p1", on: true, child: Component(name: "c1"))
Behavior(name: "b1", parent: p1).Start()

p2 = Component(name: "p2", on: true, child: Component(name: "c2", on: true))
Behavior(name: "b2", parent: p2).Start()

p3 = Component(name: "p3", on: false, child: Component(name: "c3"))
Behavior(name: "b3", parent: p3).Start()

Behavior(name: "b4").Start()

