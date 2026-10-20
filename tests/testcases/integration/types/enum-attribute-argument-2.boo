"""
Kind B True
Small Y True
B True
"""
import System
import System.Reflection

enum Kind:
	A
	B = 7

enum Small:
	X
	Y

class TagAttribute(System.Attribute):
	public Value as object

	public def constructor(kind as Kind, value as object):
		Value = value

class MarkAttribute(System.Attribute):
	public Kind as Kind

	public def constructor(kind as Kind):
		Kind = kind

[Tag(Kind.A, Kind.B)]
class One:
	pass

[Tag(Kind.A, Small.Y)]
class Two:
	pass

for t in (One, Two):
	a as TagAttribute = Attribute.GetCustomAttribute(t, TagAttribute)
	print a.Value.GetType().Name, a.Value, a.Value isa Enum

mark as MarkAttribute = Attribute.GetCustomAttribute(typeof(One).Assembly, MarkAttribute)
print mark.Kind, mark.Kind == Kind.B

[assembly: Mark(Kind.B)]
