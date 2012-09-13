class QuackTest(IQuackFu):
	private i as int
	def constructor(j as int):
		i = j
	def QuackInvoke(name as string, args as (object)) as object:
		assert name == "op_UnaryNegation"
		assert 1 == len(args)
		return QuackTest(- cast(QuackTest, args[0]).i)
	def QuackSet(name as string, parameters as (object), value) as object:
		assert parameters is null
		i = value
	def QuackGet(name as string, parameters as (object)) as object:
		assert parameters is null
		return i

_quacker as duck = QuackTest(17)
q = -_quacker
assert -17 == q.i

_duck as duck = 10
_duck = -_duck
assert _duck == -10

_1ul as ulong = 1
_2ul as ulong = 2
_ulong as ulong = 1
for i in range(5):
	_ulong *= _2ul
_ulong -= _1ul

_decimal as decimal = 1
for i in range(5):
	_decimal *= 2
_decimal -= 1
_decimal *= -1

_duck = _ulong
_duck = -_duck
assert _decimal == _duck
assert long == _duck.GetType()

_uint as uint = (2.0**32 - 1)
_duck = _uint
_duck = -_duck
assert _duck == -(2.0**32 - 1)
assert long == _duck.GetType()

_ushort as ushort = (2.0**16 - 1)
_duck = _ushort
_duck = -_duck
assert _duck == -(2.0**16 - 1)
assert int == _duck.GetType()
