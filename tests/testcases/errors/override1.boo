"""
override1.boo(6,14): BCE0060: 'Foo.Bar()': no suitable method found to override.
override1.boo(7,9): BCE0061: 'Foo.Bar()' is not an override.
"""
class Foo:
	override def Bar():
		super()

