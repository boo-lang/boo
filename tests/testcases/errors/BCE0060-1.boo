"""
BCE0060-1.boo(6,18): BCE0060: 'Foo.Bar()': no suitable method found to override.
BCE0060-1.boo(7,9): BCE0061: 'Foo.Bar()' is not an override.
"""
class Foo:
	override def Bar():
		super()

