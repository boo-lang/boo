"""
One B A X 0 -
Two A B Y 5 -
Three B A Y 2 One
"""
import System

enum Kind:
	A
	B = 7

enum Small:
	X
	Y

class KindAttribute(System.Attribute):
	[Getter(Value)]
	_value as Kind

	public Extra as Kind
	public Level as Small
	public Target as Type
	public Count as int

	public def constructor(k as Kind):
		_value = k

	public def constructor(k as Kind, small as Small, t as Type, *more as (Kind)):
		_value = k
		Level = small
		Target = t
		Count = more.Length

[Kind(Kind.B)]
class One:
	pass

[Kind(Kind.A, Extra: Kind.B, Level: Small.Y, Count: 5)]
class Two:
	pass

[Kind(Kind.B, Small.Y, typeof(One), Kind.A, Kind.B)]
class Three:
	pass

for t in (One, Two, Three):
	a as KindAttribute = Attribute.GetCustomAttribute(t, KindAttribute)
	print t.Name, a.Value, a.Extra, a.Level, a.Count, (a.Target.Name if a.Target is not null else "-")
