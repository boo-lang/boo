interface IInterface:
	pass

class SomeClass (IInterface):
	pass

struct SomeStruct (IInterface):
	foo as int

static class DefaultValues:

	def GP[of T]() as T:
		pass

	def ClassGP[of T(class)]() as T:
		pass

	def StructGP[of T(struct)]() as T:
		pass

	def SomeClassGP[of T(SomeClass)]() as T:
		pass

	def SomeStructGP[of T(SomeStruct)]() as T:
		pass

	def InterfaceGP[of T(IInterface)]() as T:
		pass

	def SomeClass() as SomeClass:
		pass

	def SomeStruct() as SomeStruct:
		pass

	def Interface() as IInterface:
		pass

	def Object() as object:
		pass

	def Byte() as byte:
		pass

	def SByte() as sbyte:
		pass

	def Char() as char:
		pass

	def Short() as short:
		pass

	def UShort() as ushort:
		pass

	def Int() as int:
		pass

	def UInt() as uint:
		pass

	def Long() as long:
		pass

	def ULong() as ulong:
		pass

	def Single() as single:
		pass

	def Double() as double:
		pass

	def Decimal() as decimal:
		pass

	def LeaveImplicit(x as int) as int:
		try:
			if x > 0:
				return
			elif x < 0:
				raise "oops"
		except:
			return -1
		return 42

	def LeaveVoid(x as bool) as void:
		try:
			if x:
				return
		except:
			pass

	def LeaveObject(x as bool) as object:
		try:
			if x:
				return object()
		except:
			pass

	def LeaveInt(x as bool) as int:
		try:
			if x:
				return 42
		except:
			pass

	def RaiseNothing() as int:
		raise "foo"

	def RaiseImplicit(x as int) as int:
		if x == 0:
			return
		if x == 1:
			return 1
		raise "foo"

	def SemiImplicit() as object:
		LeaveVoid(true)
		return

	def IntEnsure(x as int):
		try:
			if x == 0:
				raise "oops"
			return 1
		ensure:
			pass


assert 0 == DefaultValues.GP[of int]()
assert null == DefaultValues.ClassGP[of SomeClass]()
assert SomeStruct() == DefaultValues.StructGP[of SomeStruct]()
assert null == DefaultValues.SomeClassGP[of SomeClass]()
assert SomeStruct() == DefaultValues.SomeStructGP[of SomeStruct]()

assert not DefaultValues.InterfaceGP[of SomeClass]()
assert SomeStruct() == DefaultValues.InterfaceGP[of SomeStruct]()

assert null == DefaultValues.SomeClass()
assert SomeStruct() == DefaultValues.SomeStruct()
assert null == DefaultValues.Interface()

assert null == DefaultValues.Object()
assert 0 == DefaultValues.Char()
assert 0 == DefaultValues.Byte()
assert 0 == DefaultValues.SByte()
assert 0 == DefaultValues.Short()
assert 0 == DefaultValues.UShort()
assert 0 == DefaultValues.Int()
assert 0 == DefaultValues.UInt()
assert 0 == DefaultValues.Long()
assert 0 == DefaultValues.ULong()
assert 0 == DefaultValues.Single()
assert 0 == DefaultValues.Double()
assert 0 == DefaultValues.Decimal()
assert -1 == DefaultValues.LeaveImplicit(-1)
assert 42 == DefaultValues.LeaveImplicit(0)
assert null == DefaultValues.LeaveObject(false)
assert 0 == DefaultValues.LeaveInt(false)
assert 0 == DefaultValues.LeaveImplicit(1)
assert null != DefaultValues.LeaveObject(true)
assert 42 == DefaultValues.LeaveInt(true)
assert 0 == DefaultValues.RaiseImplicit(0)
assert 1 == DefaultValues.RaiseImplicit(1)
assert null == DefaultValues.SemiImplicit()
assert 1 == DefaultValues.IntEnsure(1)

