namespace ThrowAway

import System
import NUnit.Framework

[TestFixture]
class EnumOperationsTestFixture:
"""
Tests the various operations that can be performed on
an enum, including casting, bitwise and/or, and ones 
compliment.
"""
	def constructor():
		pass
		
	enum TestEnum:
		Zero = 0
		One = 1
		Two = 2
		Three = 3
		Four = 4

	[Test]
	def DefaultUnderlyingTypeIsInt32():
		assert Enum.GetUnderlyingType(TestEnum) == typeof(int)

	[Test]
	def IntegerCastsToEnum():
		val = 2
		valEnum = cast(TestEnum, val)
		assert valEnum == TestEnum.Two
		assert Convert.ToInt32(valEnum) == 2
	
	[Test]
	def EnumAllowsUndefinedValues():
		negOne = cast(TestEnum, -1)
		assert Convert.ToInt32(negOne) == -1
		assert negOne.GetType() == typeof(TestEnum)
	
	[Test]
	def BitwiseOr():
		one = TestEnum.One
		two = TestEnum.Two
		three = one | two
		assert three == TestEnum.Three
		assert three.GetType() == typeof(TestEnum)
		
	[Test]
	def BitwiseAnd():
		one = TestEnum.One
		two = TestEnum.Two
		zero = one & two
		assert zero == TestEnum.Zero
		assert zero.GetType() == typeof(TestEnum)
	
	[Test]
	def ExclusiveOr():
		one = TestEnum.One | TestEnum.Two
		two = TestEnum.Two | TestEnum.Four
		test = one ^ two
		assert test == TestEnum.One | TestEnum.Four
		assert test.GetType() == typeof(TestEnum)

	[Test]
	def OnesCompliment():
		zero = TestEnum.Zero
		negOne = ~zero
		assert negOne == cast(TestEnum, -1)
		assert Convert.ToInt32(negOne) == -1
		assert negOne.GetType() == typeof(TestEnum)
		
	[Test]
	def AdditionWithIntegerRValue():
		one = TestEnum.One
		two = 2
		three = one + two
		assert three == TestEnum.One | TestEnum.Two
		assert three.GetType() == typeof(TestEnum)
		
	[Test]
	def AdditionWithIntegerLValue():
		one = 1
		two = TestEnum.Two
		three = one + two
		assert three == TestEnum.One | TestEnum.Two
		assert three.GetType() == typeof(TestEnum)
		
	[Test]
	def SubtractionBetweenEnums():
		one = TestEnum.One
		two = TestEnum.Two
		val = two - one
		assert val == 1
		assert val.GetType() == Enum.GetUnderlyingType(TestEnum)
		
	[Test]
	def SubtractionWithIntegerRValue():
		one = 1
		two = TestEnum.Two
		val = two - one
		assert val == TestEnum.One
		assert val.GetType() == typeof(TestEnum)

//	[Test]
//	def MatchBetweenEnums():
//		testVal = TestEnum.One | TestEnum.Four
//		assert testVal ~= TestEnum.None
//		assert testVal ~= TestEnum.One
//		assert testVal ~= TestEnum.Four
//		assert testVal ~= TestEnum.One | TestEnum.Four
//		assert testVal !~ TestEnum.Two
//		assert testVal !~ TestEnum.One | TestEnum.Two
//		assert testVal !~ TestEnum.Two | TestEnum.Four
//		assert testVal !~ TestEnum.One | TestEnum.Two | TestEnum.Four
