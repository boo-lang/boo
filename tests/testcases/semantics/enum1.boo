"""
[System.FlagsAttribute]
public enum Bits:

	Foo = 1

	Bar = 4

	Baz = 16

	BarBaz = 20
"""
[System.Flags]
enum Bits:
	Foo = 1
	Bar = 1 << 2
	Baz = 1 << 4
	BarBaz = Bar | Baz
