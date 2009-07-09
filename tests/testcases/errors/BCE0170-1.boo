"""
BCE0170-1.boo(12,5): BCE0170: An enum member must be a constant integer value.
BCE0170-1.boo(14,5): BCE0170: An enum member must be a constant integer value.
BCE0170-1.boo(15,5): BCE0170: An enum member must be a constant integer value.
"""


enum Enum:
	Foo = 1
	Bar = 1 << 1
	PiInt = cast(int, System.Math.PI) * 2
	Pi = System.Math.PI #!
	Bar2 = Bar | PiInt & Foo
	Bar4 = Bar | Pi #!
	Same = Same #!

